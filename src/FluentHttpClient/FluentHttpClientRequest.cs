using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace FluentHttpClient
{
    public class FluentHttpClientRequest
    {
        private FluentHttpClientRequest(IFluentHttpClientRequestBuilder fluentHttpClientRequestBuilder,
            FluentHttpClient fluentHttpClient)
        {
            Message = fluentHttpClientRequestBuilder.Message;
            ReturnType = fluentHttpClientRequestBuilder.ReturnType;
            Body = fluentHttpClientRequestBuilder.Body;
            Method = fluentHttpClientRequestBuilder.Method;
            Uri = fluentHttpClientRequestBuilder.Uri;
            FluentHttpClient = fluentHttpClient;
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


        public FluentHttpClient FluentHttpClient { get; }

        public HttpRequestMessage Message { get; }
        public Type ReturnType { get; }


        public static FluentHttpClientRequestBuilder CreateNewRequest(FluentHttpClient fluentHttpClient)
        {
            return new FluentHttpClientRequestBuilder(fluentHttpClient);
        }


        private interface IFluentHttpClientRequestBuilder
        {
            Uri Uri { get; }
            HttpMethod Method { get; }
            HttpRequestMessage Message { get; }
            HttpContent Body { get; }

            Type ReturnType { get; }
        }

        public class FluentHttpClientRequestBuilder : IFluentHttpClientRequestBuilder
        {
            private HttpContent _body;
            private readonly FluentHttpClient _fluentHttpClient;
            private HttpRequestMessage _message;

            private HttpMethod _method;
            private Type _returnType;
            private Uri _uri;


            public FluentHttpClientRequestBuilder(FluentHttpClient fluentHttpClient)
            {
                _fluentHttpClient = fluentHttpClient;
            }

            Uri IFluentHttpClientRequestBuilder.Uri => _uri;

            HttpMethod IFluentHttpClientRequestBuilder.Method => _method;

            HttpRequestMessage IFluentHttpClientRequestBuilder.Message => _message;

            HttpContent IFluentHttpClientRequestBuilder.Body => _body;
            Type IFluentHttpClientRequestBuilder.ReturnType => _returnType;


            public FluentHttpClientRequestBuilder ReturnAs<T>()
            {
                _returnType = typeof(T);
                return this;
            }


            public FluentHttpClientRequestBuilder WithMethod(HttpMethod method)
            {
                _method = method;
                return this;
            }


            public FluentHttpClientRequestBuilder WithBodyContent(HttpContent body)
            {
                _body = body;
                return this;
            }


            public FluentHttpClientRequestBuilder WithBody<T>(T body, MediaTypeFormatter formatter,
                string mediaType = null)
            {
                return WithBodyContent(new ObjectContent<T>(body, formatter, mediaType));
            }

            public FluentHttpClientRequestBuilder WithBody(object body, MediaTypeFormatter formatter,
                string mediaType = null)
            {
                return WithBodyContent(new ObjectContent(body.GetType(), body, formatter, mediaType));
            }

            public FluentHttpClientRequestBuilder WithBody<T>(T body, MediaTypeHeaderValue contentType = null)
            {
                var formatter = _fluentHttpClient.FluentFormatterOption.GetFormatter(contentType);
                var mediaType = contentType?.MediaType;
                return WithBody(body, formatter, mediaType);
            }

            public FluentHttpClientRequestBuilder WithBody(object body, MediaTypeHeaderValue contentType = null)
            {
                var formatter = _fluentHttpClient.FluentFormatterOption.GetFormatter(contentType);
                var mediaType = contentType?.MediaType;
                return WithBody(body, formatter, mediaType);
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
                return new FluentHttpClientRequest(this, _fluentHttpClient);
            }
        }
    }
}