using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api.Response;

namespace Orcus.Server.OrcusSockets
{
    public class DefaultOrcusResponse : OrcusResponse
    {
        private bool _hasStarted;
        private readonly List<(Func<object, Task> callback, object state)> _startingDelegates;
        private readonly List<(Func<object, Task> callback, object state)> _completedDelegates;

        public DefaultOrcusResponse(int requestId)
        {
            RequestId = requestId;
            _startingDelegates = new List<(Func<object, Task> callback, object state)>();
            _completedDelegates = new List<(Func<object, Task> callback, object state)>();
        }

        public int RequestId { get; }

        public override int StatusCode { get; set; } = (int) HttpStatusCode.OK;
        public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
        public override Stream Body { get; set; }

        public override long? ContentLength
        {
            get => Headers.ContentLength;
            set => Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => Headers[HeaderNames.ContentType];
            set => Headers[HeaderNames.ContentType] = value;
        }

        public override bool HasStarted => _hasStarted;

        /// <summary>
        ///     Tells if the response is completed and no new data will be written to the <see cref="Body" />
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        ///     Tells if the <code>OpCodeModifier.IsFinished</code> was sent
        /// </summary>
        public bool IsFinished { get; private set; }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            _startingDelegates.Add((callback, state));
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            _completedDelegates.Add((callback, state));
        }

        public void StartResponse()
        {
            _hasStarted = true;

            foreach (var (callback, state) in _startingDelegates)
                callback.Invoke(state);
        }

        public void Finished()
        {
            IsFinished = true;

            foreach (var (callback, state) in _completedDelegates)
                callback.Invoke(state);

            Body.Dispose();
        }
    }
}