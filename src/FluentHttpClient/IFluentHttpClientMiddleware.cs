using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public delegate Task<FluentHttpClientResponse> FluentHttpClientRequestDelegate(FluentHttpClientRequest request,
        CancellationToken cancellationToken = default(CancellationToken));

    public interface IFluentHttpClientMiddleware
    {
        Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }


    public abstract class FluentHttpClientMiddleware : IFluentHttpClientMiddleware
    {
        protected FluentHttpClientMiddleware(FluentHttpClientRequestDelegate next)
        {
            Next = next;
        }


        protected FluentHttpClientRequestDelegate Next { get; set; }


        public abstract Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }


    public class NlogFluentHttpClientMiddleware : FluentHttpClientMiddleware
    {
        public NlogFluentHttpClientMiddleware(FluentHttpClientRequestDelegate next) : base(next)
        {
        }


        public override async Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine("what?!");
            return await Next(request, cancellationToken);
        }
    }


    public class Nlog2FluentHttpClientMiddleware : FluentHttpClientMiddleware
    {
        public Nlog2FluentHttpClientMiddleware(FluentHttpClientRequestDelegate next) : base(next)
        {
        }


        public override async Task<FluentHttpClientResponse> InvokeAsync(FluentHttpClientRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine("what the ...?!");
            return await Next(request, cancellationToken);
        }
    }
}