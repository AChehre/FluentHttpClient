using System.Net.Http;
using System.Net.Http.Headers;

namespace FluentHttpClient.Client
{
    public static class HttpClientExtensions
    {
        public const string Bearer = "Bearer";

        public static void SetBasicAuthentication(this HttpClient client, string userName, string password)
        {
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
        }

        public static void SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.SetToken(Bearer, token);
        }


        public static void SetBasicAuthentication(this HttpRequestHeaders defaultRequestHeaders, string userName,
            string password)
        {
            defaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
        }

        public static void SetToken(this HttpRequestHeaders defaultRequestHeaders, string scheme, string token)
        {
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }


        public static void SetBearerToken(this HttpRequestHeaders defaultRequestHeaders, string token)
        {
            defaultRequestHeaders.SetToken(Bearer, token);
        }
    }
}