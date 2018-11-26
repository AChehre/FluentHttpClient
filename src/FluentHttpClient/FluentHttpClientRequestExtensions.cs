using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientRequestExtensions
    {
        public static async Task<T> Send<T>(this FluentHttpClientRequest request)
        {
            var response = await request.FluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }

        public static async Task<T> Send<T>(this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            var request = builder.Build();
            return await Send<T>(request);
        }
    }
}