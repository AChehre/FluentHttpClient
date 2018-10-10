using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientExtensions
    {
        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder CreateNewRequest(
            this FluentHttpClient fluentHttpClient)
        {
            return FluentHttpClientRequest.CreateNewRequest(fluentHttpClient);
        }


        public static async Task<T> PostAsync<T>(this FluentHttpClient fluentHttpClient, string uri, object body,
            MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPost().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }

        public static async Task<T> PutAsync<T>(this FluentHttpClient fluentHttpClient, string uri, object body,
            MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPut().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }


        public static async Task<T> DeleteAsync<T>(this FluentHttpClient fluentHttpClient, string uri, object body,
            MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsDelete().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }


        public static async Task<T> PatchAsync<T>(this FluentHttpClient fluentHttpClient, string uri, object body,
            MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPatch().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }


        public static async Task<FluentHttpClientResponse<T>> GetFluentAsync<T>(this FluentHttpClient fluentHttpClient,
            string uri)
        {
            var request = fluentHttpClient.CreateNewRequest().AsGet().WithUri(uri).Build();
            return await fluentHttpClient.SendAsync<T>(request);
        }

        public static async Task<FluentHttpClientResponse> GetAsync(this FluentHttpClient fluentHttpClient,
            string uri)
        {
            var request = fluentHttpClient.CreateNewRequest().AsGet().WithUri(uri).Build();
            return await fluentHttpClient.SendAsync(request);
        }


        public static async Task<T> GetAsync<T>(this FluentHttpClient fluentHttpClient, string uri)
        {
            var response = await GetFluentAsync<T>(fluentHttpClient, uri);

            return response.Content;
        }
    }
}