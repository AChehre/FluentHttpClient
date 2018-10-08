using System.Net;
using System.Net.Http;

namespace FluentHttpClient
{
    public class FluentHttpClientResponse
    {
        public readonly HttpResponseMessage Message;

        public FluentHttpClientResponse(HttpResponseMessage message)
        {
            Message = message;
        }

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


        public void EnsureSuccessStatusCode()
        {
            Message.EnsureSuccessStatusCode();
        }
    }


    public class FluentHttpClientResponse<T> : FluentHttpClientResponse
    {
        public FluentHttpClientResponse(HttpResponseMessage message) : base(message)
        {
        }

        public T Content { get; set; }
    }
}