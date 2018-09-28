using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentHttpClient
{

    public delegate Task<HttpResponseMessage> IFluentHttpClientRequestDelegate(FluentHttpClientRequest request);


    public interface IFluentHttpClientMiddleware
    {
        Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request, IFluentHttpClientRequestDelegate next);
    }


    public class TestMiddleware : IFluentHttpClientMiddleware
    {
        public async Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request, IFluentHttpClientRequestDelegate next)
        {

            return await next(request);
        }
    }




    public class NLogMiddleware : IFluentHttpClientMiddleware
    {

        public async Task<HttpResponseMessage> InvokeAsync(FluentHttpClientRequest request, IFluentHttpClientRequestDelegate next)
        {

            return await next(request);
        }
    }




   
}