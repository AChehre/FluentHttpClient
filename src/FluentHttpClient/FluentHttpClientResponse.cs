using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public class FluentHttpClientResponse
    {
        public FluentHttpClientResponse(HttpResponseMessage message)
        {
            Message = message;
        }

        public readonly HttpResponseMessage Message;

      
        public HttpStatusCode StatusCode
        {
            get => Message.StatusCode;
            set => Message.StatusCode = value;
        }


        public HttpContent Body
        {
            get => Message.Content;
            set => Message.Content = value;
        }

        public bool IsSuccessStatusCode => Message.IsSuccessStatusCode;

     
        public void EnsureSuccessStatusCode() => Message.EnsureSuccessStatusCode();
    }
}
