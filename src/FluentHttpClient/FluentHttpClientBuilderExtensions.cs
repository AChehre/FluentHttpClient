using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientBuilderExtensions
    {
        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsPost(this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Post);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsGet(this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Get);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsPut(this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Put);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsDelete(this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Delete);
            return builder;
        }
    }
}
