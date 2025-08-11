using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace MetarTaf_Tests
{
    public sealed class FakeHandler : HttpMessageHandler
    {
        private readonly string _payload;
        private readonly HttpStatusCode _code;
        public FakeHandler(string payload, HttpStatusCode code = HttpStatusCode.OK)
        { _payload = payload; _code = code; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = new HttpResponseMessage(_code)
            {
                Content = new StringContent(_payload)
            };
            return Task.FromResult(resp);
        }
    }

}
