using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientExtensions
    {
        public static async Task<T> PostAsJson<T>(this FluentHttpClient fluentHttpClient, string uri, object body)
        {
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            var mediaType = JsonMediaTypeFormatter.DefaultMediaType.MediaType;
            var request = FluentHttpClientRequest.CreateNewRequest().AsPost().WithUri(uri)
                .WithBody(body, jsonMediaTypeFormatter, mediaType).Build();
            var response = await fluentHttpClient.SendAsync<T>(request);
            return response.Content;
        }
    }
}