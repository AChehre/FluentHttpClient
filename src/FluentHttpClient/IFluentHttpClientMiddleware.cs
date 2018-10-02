using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public delegate Task<FluentHttpClientResponse> FluentHttpClientRequestDelegate(FluentHttpClientRequest request);
    public interface IFluentHttpClientMiddleware
    {
        Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request);
    }


    public abstract class FluentHttpClientMiddleware : IFluentHttpClientMiddleware
    {
        protected FluentHttpClientMiddleware(FluentHttpClientRequestDelegate next)
        {
            Next = next;
        }


        protected FluentHttpClientRequestDelegate Next { get; set; }


        public abstract Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request);
    }





    public class NlogFluentHttpClientMiddleware : FluentHttpClientMiddleware
    {
        public NlogFluentHttpClientMiddleware(FluentHttpClientRequestDelegate next) :base(next)
        {
        
        }


     
        public override async Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request)
        {
           Debug.WriteLine("what?!");
           return  await Next(request);
        }
    }


    public class Nlog2FluentHttpClientMiddleware : FluentHttpClientMiddleware
    {
        public Nlog2FluentHttpClientMiddleware(FluentHttpClientRequestDelegate next) : base(next)
        {

        }



        public override async Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request)
        {
            Debug.WriteLine("what the ...?!");
            return await Next(request);
        }
    }



}