using System.Net.Http.Formatting;
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


        public static async Task<T> Post<T>(this FluentHttpClient fluentHttpClient, string uri, object body, MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPost().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }

        public static async Task<T> Put<T>(this FluentHttpClient fluentHttpClient, string uri, object body, MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPut().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }


        public static async Task<T> Delete<T>(this FluentHttpClient fluentHttpClient, string uri, object body, MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsDelete().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }


        public static async Task<T> Patch<T>(this FluentHttpClient fluentHttpClient, string uri, object body, MediaTypeHeaderValue contentType = null)
        {
            var request = fluentHttpClient.CreateNewRequest().AsPatch().WithUri(uri)
                .WithBody(body, contentType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }






        public static async Task<T> GetAsJson<T>(this FluentHttpClient fluentHttpClient, string uri)
        {
            var request = fluentHttpClient.CreateNewRequest().AsGet().WithUri(uri).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }
    }
}