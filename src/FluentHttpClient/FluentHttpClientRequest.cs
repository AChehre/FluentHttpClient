using System;
using System.Net.Http;

namespace FluentHttpClient
{
    public class FluentHttpClientRequest
    {

        public FluentHttpClientRequest(HttpRequestMessage message)
        {
            Message = message;
        }

        public Uri Uri
        {
            get => Message.RequestUri;
            set => Message.RequestUri = value;
        }

        public object Data { get; set; }

        public HttpMethod Method
        {
            get => Message.Method;
            set => Message.Method = value;
        }

        public HttpRequestMessage Message { get; }
    }
}