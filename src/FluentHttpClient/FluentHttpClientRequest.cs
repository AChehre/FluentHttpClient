using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public class FluentHttpClientRequest
    {
        public string Uri { get; set; }
        public object Data { get; set; }
        public HttpMethod Method { get; set; }
    }
}
