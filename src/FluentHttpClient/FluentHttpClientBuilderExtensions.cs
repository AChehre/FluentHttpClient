namespace FluentHttpClient
{
    public static class FluentHttpClientBuilderExtensions
    {
        public static FluentHttpClient.FluentHttpClientBuilder WithJsonDefaultRequestHeaders(this FluentHttpClient.FluentHttpClientBuilder builder)
        {
            return builder.AddAcceptHeader(MimeTypes.Application.Json);
        }
    }
}