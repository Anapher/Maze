using RemoteDesktop.Client.Capture.DesktopDuplication;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Client.Encoder.x264;
using x264net;

namespace RemoteDesktop.TestApp
{
    public partial class Form1 : Form
    {
        private DesktopDuplicationCaptureService _service;
        private IStreamEncoder _encoder;
        private int _fps;
        private DateTimeOffset _offset;
        private DateTimeOffset _dataOffset;
        private int _data;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _offset = DateTimeOffset.UtcNow;

            Task.Run(() =>
            {
                _service = new DesktopDuplicationCaptureService();
                var screenInfo = _service.Initialize(0);
                _encoder = new TestEncoder(screenInfo) { SetImage = ImportData };

                while (true)
                {
                    _service.Capture(_encoder);
                    _fps++;

                    var diff = (DateTimeOffset.UtcNow - _offset);
                    if (diff.TotalSeconds >= 1)
                    {
                        var fps = _fps;
                        var data = _data;

                        _data = 0;
                        _fps = 0;
                        _offset = DateTimeOffset.UtcNow;

                        BeginInvoke(new Action(() => { this.Text = $"{fps} FPS ({data / 1024} KiB/s)"; }));
                    }
                }
            });
        }

        private void SetImage(Bitmap obj)
        {
            Invoke((Action)(() =>
            {
                var image = pictureBox1.Image;
                pictureBox1.Image = obj;
                image?.Dispose();
            }));
        }

        private OpenH264Lib.Decoder decoder = new OpenH264Lib.Decoder(@"F:\Projects\Orcus\src\modules\RemoteDesktop\test\RemoteDesktop.TestApp\bin\Debug\openh264-1.8.0-win32.dll");

        private Task ImportData(byte[] data, int length)
        {
            _data += length;
            return Task.Run(() =>
            {
                var decode = decoder.Decode(data, length);
                BeginInvoke(new Action(() => pictureBox1.Image = decode));
            });
        }
    }

    public class TestEncoder : IStreamEncoder
    {
        private readonly ScreenInfo _screenInfo;
        private X264Net _x264Net;
        private readonly FileStream _fileStream;
        private readonly Bitmap _sourceImage;

        public TestEncoder(ScreenInfo screenInfo)
        {
            _screenInfo = screenInfo;
            _x264Net = new X264Net(1920, 1080);

            _sourceImage = new Bitmap(screenInfo.Width, screenInfo.Height);
            _fileStream = File.Create("test.h264");
        }

        public Func<byte[], int, Task> SetImage { get; set; }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe IntPtr memcpy(byte* dst, byte* src, UIntPtr count);

        public void Initialize(ScreenInfo screenInfo, IFrameTransmitter transmitter)
        {
            throw new NotImplementedException();
        }

        private readonly object _lock = new object();

        public unsafe void CodeImage(IntPtr scan0, PixelFormat pixelFormat)
        {
            //var bmpData = _sourceImage.LockBits(new Rectangle(Point.Empty, _sourceImage.Size), ImageLockMode.WriteOnly, pixelFormat);
            //memcpy(bmpData.Scan0, scan0, (UIntPtr)(bmpData.Stride * bmpData.Height));
            //_sourceImage.UnlockBits(bmpData);

            //var sw1 = Stopwatch.StartNew();
            //var bmp = ResizeImage(_sourceImage, 1280, 720);
            //var e = sw1.ElapsedMilliseconds;
            //using (bmp)
            //{
            //    var lockBits = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, pixelFormat);
            //    try
            //    {
                    var pointer = scan0.ToPointer();
                    var result = _x264Net.EncodeFrame(pointer).Cast<X264Nal>().ToArray();

                    var length = result.Sum(x => x.Length);
                    var data = ArrayPool<byte>.Shared.Rent(length);
                    try
                    {
                        var position = 0;
                        fixed (byte* dataPointer = data)
                        {
                            foreach (var x264Nal in result)
                            {
                                memcpy(dataPointer + position, x264Nal.Pointer, (UIntPtr)x264Nal.Length);
                                position += x264Nal.Length;
                            }
                        }

                        SetImage(data, length).ContinueWith(task => ArrayPool<byte>.Shared.Return(data));
                        //_fileStream.Write(data, 0, length);
                    }
                    finally
                    {
                    }
                //}
                //finally
                //{
                //    bmp.UnlockBits(lockBits);
                //}
            //}

            //_fileStream.Write(result, 0, result.Length);
            //new x264net.X264Frame { }


            //var bitmap = new Bitmap(_screenInfo.Width, _screenInfo.Height, pixelFormat);
            //var lockBits =bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, pixelFormat);
            //try
            //{
            //    memcpy(lockBits.Scan0, scan0, new UIntPtr((uint) (lockBits.Height * 4 * lockBits.Width)));
            //}
            //finally
            //{
            //    bitmap.UnlockBits(lockBits);
            //}

            //Task.Run(() => SetImage.Invoke(bitmap));
        }

        //public static unsafe byte[] ResizeImage(IntPtr scan0, int sourceHeight, int sourceWidth, int destinationWidth, int destinationHeight, PixelFormat pixelFormat)
        //{
        //    var result = new byte[destinationHeight * destinationWidth * 4];

        //    var tx = sourceWidth / (double) destinationWidth;
        //    var ty = 
        //}

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
