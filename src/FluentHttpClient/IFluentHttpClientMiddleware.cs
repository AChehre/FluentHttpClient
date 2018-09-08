using System;
using System.Net.Http;

namespace FluentHttpClient
{

    public class NLogMiddleware : FluentHttpClientMiddleware
    {

        //private readonly ILogger _logger;

        public NLogMiddleware(FluentHttpClientMiddleware next) : base(next)
        {
        }


        public override HttpResponseMessage OnRequest(FluentHttpClientRequest request)
        {
            return Next.OnRequest(request);
        }
    }




    public abstract class FluentHttpClientMiddleware //: IFluentHttpClientMiddleware
    {

        protected readonly FluentHttpClientMiddleware Next;

        protected FluentHttpClientMiddleware(FluentHttpClientMiddleware next)
        {
            Next = next;
        }

        public abstract HttpResponseMessage OnRequest(FluentHttpClientRequest request);
    }




    public class FluentHttpClientRequest
    {
        public string Uri { get; set; }
        public object Data { get; set; }
    }
}