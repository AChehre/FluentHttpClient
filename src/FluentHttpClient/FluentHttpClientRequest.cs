using System;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace FluentHttpClient
{
    public class FluentHttpClientRequest
    {
        private FluentHttpClientRequest(IFluentHttpClientRequestBuilder fluentHttpClientRequestBuilder)
        {
            Message = fluentHttpClientRequestBuilder.Message;
        }

        public Uri Uri
        {
            get => Message.RequestUri;
            set => Message.RequestUri = value;
        }

        public HttpMethod Method
        {
            get => Message.Method;
            set => Message.Method = value;
        }

        public HttpContent Body
        {
            get => Message.Content;
            set => Message.Content = value;
        }


        public HttpRequestMessage Message { get; }


        public static FluentHttpClientRequestBuilder CreateNewRequest()
        {
            return new FluentHttpClientRequestBuilder();
        }


        private interface IFluentHttpClientRequestBuilder
        {
            Uri Uri { get; }
            HttpMethod Method { get; }
            HttpRequestMessage Message { get; }
            HttpContent Body { get; }

            Type ReturnAs { get; }
        }

        public class FluentHttpClientRequestBuilder : IFluentHttpClientRequestBuilder
        {
            private HttpContent _body;
            private HttpRequestMessage _message;

            private HttpMethod _method;
            private Type _returnAs;
            private Uri _uri;

            Uri IFluentHttpClientRequestBuilder.Uri => _uri;

            HttpMethod IFluentHttpClientRequestBuilder.Method => _method;

            HttpRequestMessage IFluentHttpClientRequestBuilder.Message => _message;

            HttpContent IFluentHttpClientRequestBuilder.Body => _body;
            Type IFluentHttpClientRequestBuilder.ReturnAs => _returnAs;


            public FluentHttpClientRequestBuilder ReturnAs<T>()
            {
                _returnAs = typeof(T);
                return this;
            }


            public FluentHttpClientRequestBuilder WithMethod(HttpMethod method)
            {
                _method = method;
                return this;
            }

            public FluentHttpClientRequestBuilder AsPost()
            {
                _method = HttpMethod.Post;
                return this;
            }

            public FluentHttpClientRequestBuilder AsGet()
            {
                _method = HttpMethod.Get;
                return this;
            }

            public FluentHttpClientRequestBuilder AsPut()
            {
                _method = HttpMethod.Put;
                return this;
            }

            public FluentHttpClientRequestBuilder AsDelete()
            {
                _method = HttpMethod.Delete;
                return this;
            }

            public FluentHttpClientRequestBuilder WithBodyContent(HttpContent body)
            {
                _body = body;
                return this;
            }



            public FluentHttpClientRequestBuilder WithBody<T>(T body, MediaTypeFormatter formatter, string mediaType = null)
            {
                return WithBodyContent(new ObjectContent<T>(body, formatter, mediaType));
            }

            public FluentHttpClientRequestBuilder WithUri(Uri uri)
            {
                _uri = uri;
                return this;
            }

            public FluentHttpClientRequestBuilder WithUri(string uri)
            {
                _uri = new Uri(uri);
                return this;
            }

            public FluentHttpClientRequest Build()
            {
                _message.RequestUri = _uri;
                _message.Method = _method;
                _message.Content = _body;
                return new FluentHttpClientRequest(this);
            }
        }
    }
}