using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentHttpClient
{
    public static class FluentHttpClientResponseExtensions
    {
        public static async Task<T> AsJson<T>(this FluentHttpClientResponse response)
        {
            var stringResult = await response.Message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }

    }
}
