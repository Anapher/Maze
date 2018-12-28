using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Service.Commander.Infrastructure
{
    /// <summary>
    ///     A Stream that wraps another stream and enables rewinding by buffering the content as it is read.
    ///     The content is buffered in memory up to a certain size and then spooled to a temp file on disk.
    ///     The temp file will be deleted on Dispose.
    /// </summary>
    public class FileBufferingReadStream : Stream
    {
        private const int _maxRentedBufferSize = 1024 * 1024; // 1MB
        private readonly long? _bufferLimit;
        private readonly ArrayPool<byte> _bytePool;
        private readonly Stream _inner;
        private readonly int _memoryThreshold;
        private readonly Func<string> _tempFileDirectoryAccessor;

        private Stream _buffer;
        private bool _completelyBuffered;

        private bool _disposed;
        private byte[] _rentedBuffer;
        private string _tempFileDirectory;

        public FileBufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit,
            Func<string> tempFileDirectoryAccessor) : this(inner, memoryThreshold, bufferLimit,
            tempFileDirectoryAccessor, ArrayPool<byte>.Shared)
        {
        }

        public FileBufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit,
            Func<string> tempFileDirectoryAccessor, ArrayPool<byte> bytePool)
        {
            if (inner == null) throw new ArgumentNullException(nameof(inner));

            if (tempFileDirectoryAccessor == null) throw new ArgumentNullException(nameof(tempFileDirectoryAccessor));

            _bytePool = bytePool;
            if (memoryThreshold < _maxRentedBufferSize)
            {
                _rentedBuffer = bytePool.Rent(memoryThreshold);
                _buffer = new MemoryStream(_rentedBuffer);
                _buffer.SetLength(0);
            }
            else
            {
                _buffer = new MemoryStream();
            }

            _inner = inner;
            _memoryThreshold = memoryThreshold;
            _bufferLimit = bufferLimit;
            _tempFileDirectoryAccessor = tempFileDirectoryAccessor;
        }

        public FileBufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit, string tempFileDirectory) :
            this(inner, memoryThreshold, bufferLimit, tempFileDirectory, ArrayPool<byte>.Shared)
        {
        }

        public FileBufferingReadStream(Stream inner, int memoryThreshold, long? bufferLimit, string tempFileDirectory,
            ArrayPool<byte> bytePool)
        {
            if (inner == null) throw new ArgumentNullException(nameof(inner));

            if (tempFileDirectory == null) throw new ArgumentNullException(nameof(tempFileDirectory));

            _bytePool = bytePool;
            if (memoryThreshold < _maxRentedBufferSize)
            {
                _rentedBuffer = bytePool.Rent(memoryThreshold);
                _buffer = new MemoryStream(_rentedBuffer);
                _buffer.SetLength(0);
            }
            else
            {
                _buffer = new MemoryStream();
            }

            _inner = inner;
            _memoryThreshold = memoryThreshold;
            _bufferLimit = bufferLimit;
            _tempFileDirectory = tempFileDirectory;
        }

        public bool InMemory { get; private set; } = true;

        public string TempFileName { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _buffer.Length;

        public override long Position
        {
            get => _buffer.Position;
            // Note this will not allow seeking forward beyond the end of the buffer.
            set
            {
                ThrowIfDisposed();
                _buffer.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            if (!_completelyBuffered && origin == SeekOrigin.End)
                throw new NotSupportedException("The content has not been fully buffered yet.");
            if (!_completelyBuffered && origin == SeekOrigin.Current && offset + Position > Length)
                throw new NotSupportedException("The content has not been fully buffered yet.");
            if (!_completelyBuffered && origin == SeekOrigin.Begin && offset > Length)
                throw new NotSupportedException("The content has not been fully buffered yet.");
            return _buffer.Seek(offset, origin);
        }

        private Stream CreateTempFile()
        {
            if (_tempFileDirectory == null)
            {
                Debug.Assert(_tempFileDirectoryAccessor != null);
                _tempFileDirectory = _tempFileDirectoryAccessor();
                Debug.Assert(_tempFileDirectory != null);
            }

            TempFileName = Path.Combine(_tempFileDirectory, "ASPNETCORE_" + Guid.NewGuid() + ".tmp");
            return new FileStream(TempFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete, 1024 * 16,
                FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            if (_buffer.Position < _buffer.Length || _completelyBuffered)
                return _buffer.Read(buffer, offset, (int) Math.Min(count, _buffer.Length - _buffer.Position));

            var read = _inner.Read(buffer, offset, count);

            if (_bufferLimit.HasValue && _bufferLimit - read < _buffer.Length)
            {
                Dispose();
                throw new IOException("Buffer limit exceeded.");
            }

            if (InMemory && _buffer.Length + read > _memoryThreshold)
            {
                InMemory = false;
                var oldBuffer = _buffer;
                _buffer = CreateTempFile();
                if (_rentedBuffer == null)
                {
                    oldBuffer.Position = 0;
                    var rentedBuffer = _bytePool.Rent(Math.Min((int) oldBuffer.Length, _maxRentedBufferSize));
                    var copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    while (copyRead > 0)
                    {
                        _buffer.Write(rentedBuffer, 0, copyRead);
                        copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    }

                    _bytePool.Return(rentedBuffer);
                }
                else
                {
                    _buffer.Write(_rentedBuffer, 0, (int) oldBuffer.Length);
                    _bytePool.Return(_rentedBuffer);
                    _rentedBuffer = null;
                }
            }

            if (read > 0)
                _buffer.Write(buffer, offset, read);
            else
                _completelyBuffered = true;

            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (_buffer.Position < _buffer.Length || _completelyBuffered)
                return await _buffer.ReadAsync(buffer, offset, (int) Math.Min(count, _buffer.Length - _buffer.Position),
                    cancellationToken);

            var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);

            if (_bufferLimit.HasValue && _bufferLimit - read < _buffer.Length)
            {
                Dispose();
                throw new IOException("Buffer limit exceeded.");
            }

            if (InMemory && _buffer.Length + read > _memoryThreshold)
            {
                InMemory = false;
                var oldBuffer = _buffer;
                _buffer = CreateTempFile();
                if (_rentedBuffer == null)
                {
                    oldBuffer.Position = 0;
                    var rentedBuffer = _bytePool.Rent(Math.Min((int) oldBuffer.Length, _maxRentedBufferSize));
                    // oldBuffer is a MemoryStream, no need to do async reads.
                    var copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    while (copyRead > 0)
                    {
                        await _buffer.WriteAsync(rentedBuffer, 0, copyRead, cancellationToken);
                        copyRead = oldBuffer.Read(rentedBuffer, 0, rentedBuffer.Length);
                    }

                    _bytePool.Return(rentedBuffer);
                }
                else
                {
                    await _buffer.WriteAsync(_rentedBuffer, 0, (int) oldBuffer.Length, cancellationToken);
                    _bytePool.Return(_rentedBuffer);
                    _rentedBuffer = null;
                }
            }

            if (read > 0)
                await _buffer.WriteAsync(buffer, offset, read, cancellationToken);
            else
                _completelyBuffered = true;

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_rentedBuffer != null) _bytePool.Return(_rentedBuffer);

                if (disposing) _buffer.Dispose();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileBufferingReadStream));
        }
    }
}