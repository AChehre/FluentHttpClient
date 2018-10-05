using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientExtensions
    {
        public static async Task<T> PostAsJson<T>(this FluentHttpClient fluentHttpClient, string uri, T body)
        {
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            string mediaType = JsonMediaTypeFormatter.DefaultMediaType.MediaType;
            var request = FluentHttpClientRequest.CreateNewRequest().AsPost().WithUri(uri).WithBody(body, jsonMediaTypeFormatter, mediaType).ReturnAs<T>().Build();
           var a = await fluentHttpClient.SendAsync(request);
            return a.AsJson<T>();
        }
    }
}
