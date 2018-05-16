using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orcus.Administration.Core.Clients.Helpers
{
    internal class ResponseFactory<T>
    {
        private readonly Task<HttpResponseMessage> _response;
        private Action<T> _postProcessAction;
        private Func<T, T> _postProcessAction2;
        private JsonSerializerSettings _jsonSerializerSettings;

        public ResponseFactory(Task<HttpResponseMessage> response)
        {
            _response = response;
        }

        public ResponseFactory<T> PostProcess(Action<T> postProcessAction)
        {
            _postProcessAction = postProcessAction;
            return this;
        }

        public ResponseFactory<T> PostProcess(Func<T, T> postProcessAction)
        {
            _postProcessAction2 = postProcessAction;
            return this;
        }

        public ResponseFactory<T> SetJsonSettings(JsonSerializerSettings jsonSettings)
        {
            _jsonSerializerSettings = jsonSettings;
            return this;
        }

        public async Task<T> ToResult()
        {
            using (var response = await _response)
            {
                var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync(),
                    _jsonSerializerSettings ?? JsonContent.JsonSettings);
                _postProcessAction?.Invoke(result);
                if (_postProcessAction2 != null)
                    result = _postProcessAction2.Invoke(result);
                return result;
            }
        }
    }
}
