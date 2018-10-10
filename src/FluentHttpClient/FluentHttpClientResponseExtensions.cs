using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public static class FluentHttpClientResponseExtensions
    {
        public static async Task<T> As<T>(this FluentHttpClientResponse response)
        {
            return await response.Message.Content.ReadAsAsync<T>().ConfigureAwait(false);
        }

        public static async Task<string> AsString(this FluentHttpClientResponse response)
        {
            return await response.Message.Content.ReadAsStringAsync().ConfigureAwait(false);
        }


        public static async Task<byte[]> AsByteArray(this FluentHttpClientResponse response)
        {
            return await response.Message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }


        public static async Task<Stream> AsStream(this FluentHttpClientResponse response)
        {
            var stream = await response.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
            stream.Position = 0;
            return stream;
        }
    }
}