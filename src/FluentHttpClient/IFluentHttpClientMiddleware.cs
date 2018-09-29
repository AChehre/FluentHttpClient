using System.Net.Http;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public abstract class FluentHttpClientMiddleware
    {
        protected FluentHttpClientMiddleware(FluentHttpClientMiddleware next)
        {
            Next = next;
        }


        protected FluentHttpClientMiddleware Next { get; set; }


        public abstract Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request);
    }


    public class HttpClientMiddleware : FluentHttpClientMiddleware
    {
        private readonly FluentHttpClient _httpClient;

        public HttpClientMiddleware(FluentHttpClientMiddleware next, FluentHttpClient httpClient) : base(next)
        {
            _httpClient = httpClient;
        }


        public override async Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request)
        {
            if (Next != null)
            {
                return await Next.InvokeAsync(request);
            }

            return _httpClient.SendAsync(request);
        }
    }


    public class NLogMiddleware : FluentHttpClientMiddleware
    {
        public NLogMiddleware(FluentHttpClientMiddleware next) : base(next)
        {
        }


        public override async Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request)
        {
            return await Next.InvokeAsync(request);
        }
    }
}