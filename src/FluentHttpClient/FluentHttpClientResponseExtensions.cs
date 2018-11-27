using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentHttpClient
{
    public static class FluentHttpClientResponseExtensions
    {
        public static async Task<T> As<T>(this FluentHttpClientResponse response)
        {
            var streamContent = await response.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return DeserializeJsonFromStream<T>(streamContent);
        }


        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default(T);
            }

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
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