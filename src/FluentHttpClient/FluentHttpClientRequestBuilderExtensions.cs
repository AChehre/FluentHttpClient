using System.Net.Http;

namespace FluentHttpClient
{
    public static class FluentHttpClientRequestBuilderExtensions
    {
        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsPost(
            this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Post);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsGet(
            this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Get);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsPut(
            this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Put);
            return builder;
        }

        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsDelete(
            this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(HttpMethod.Delete);
            return builder;
        }


        public static FluentHttpClientRequest.FluentHttpClientRequestBuilder AsPatch(
            this FluentHttpClientRequest.FluentHttpClientRequestBuilder builder)
        {
            builder.WithMethod(new HttpMethod("PATCH"));
            return builder;
        }
    }
}