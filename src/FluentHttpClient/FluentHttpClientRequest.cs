using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace FluentHttpClient
{
    public class FluentHttpClientRequest
    {
        private FluentHttpClientRequest(IFluentHttpClientRequestBuilder fluentHttpClientRequestBuilder,
            FluentHttpClient fluentHttpClient)
        {
            Message = fluentHttpClientRequestBuilder.Message;
            FluentHttpClient = fluentHttpClient;
            EnsureSuccessStatusCode = fluentHttpClientRequestBuilder.EnsureSuccessStatusCode;
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

        public bool EnsureSuccessStatusCode { get; }


        public FluentHttpClient FluentHttpClient { get; }

        public HttpRequestMessage Message { get; }


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
            bool EnsureSuccessStatusCode { get; }
            IDictionary<string, string> QueryParams { get; }
        }

        public class FluentHttpClientRequestBuilder : IFluentHttpClientRequestBuilder
        {
            private readonly FluentHttpClient _fluentHttpClient;
            private HttpContent _body;
            private bool _ensureSuccessStatusCode;
            private HttpRequestMessage _message;

            private HttpMethod _method;
            private IDictionary<string, string> _queryParams;
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

            bool IFluentHttpClientRequestBuilder.EnsureSuccessStatusCode => _ensureSuccessStatusCode;

            IDictionary<string, string> IFluentHttpClientRequestBuilder.QueryParams => _queryParams;



            public FluentHttpClientRequestBuilder ReturnAs<T>()
            {
                _returnType = typeof(T);
                return this;
            }


            public FluentHttpClientRequestBuilder EnsureSuccessStatusCode()
            {
                _ensureSuccessStatusCode = true;
                return this;
            }


            public FluentHttpClientRequestBuilder WithQueryParams(IDictionary<string, string> queryParams)
            {
                _queryParams = queryParams;
                return this;
            }

            public FluentHttpClientRequestBuilder WithQueryParam(string key, string value)
            {
                if (_queryParams == null)
                {
                    _queryParams = new Dictionary<string, string>();
                }

                _queryParams.Add(key, value);
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


            //public FluentHttpClientRequestBuilder WithBody<T>(T body, MediaTypeFormatter formatter,
            //    string mediaType = null)
            //{
            //    return WithBodyContent();
            //}

            public FluentHttpClientRequestBuilder WithBody(object body)
            {
                return WithBodyContent(new JsonContent(body));
            }

            public FluentHttpClientRequestBuilder WithBody(object body, string mediaType)
            {
                return WithBodyContent(new JsonContent(body, mediaType));
            }


            public FluentHttpClientRequestBuilder WithPatchBody(object body)
            {
                return WithBodyContent(new PatchContent(body));
            }


         
            public FluentHttpClientRequestBuilder WithFileBody(string filePath, string apiParamName)
            {
                return WithBodyContent(new FileContent(filePath, apiParamName));
            }




            //public FluentHttpClientRequestBuilder WithBody<T>(T body, MediaTypeHeaderValue contentType = null)
            //{
            //    var formatter = _fluentHttpClient.FluentFormatterOption.GetFormatter(contentType);
            //    var mediaType = contentType?.MediaType;
            //    return WithBody(body, formatter, mediaType);
            //}

            //public FluentHttpClientRequestBuilder WithBody(object body, MediaTypeHeaderValue contentType = null)
            //{
            //    var formatter = _fluentHttpClient.FluentFormatterOption.GetFormatter(contentType);
            //    var mediaType = contentType?.MediaType;
            //    return WithBody(body, formatter, mediaType);
            //}


            public FluentHttpClientRequestBuilder WithUri(Uri uri)
            {
                _uri = uri;
                return this;
            }

            public FluentHttpClientRequestBuilder WithUri(string uri)
            {
                _uri = new Uri(uri, UriKind.Relative);
                return this;
            }


            public FluentHttpClientRequest Build()
            {
                if (_uri == null)
                {
                    WithUri("");
                }

                var uri = _uri;

                if (_queryParams != null && _queryParams.Any())
                {
                    uri = AddParameterToUri(_uri, _queryParams);
                }


                _message = new HttpRequestMessage(_method, uri) {Content = _body};

                return new FluentHttpClientRequest(this, _fluentHttpClient);
            }


            private static Uri AddParameterToUri(Uri uri, IDictionary<string, string> queryParams)
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                foreach (var queryParam in queryParams)
                {
                    queryString[queryParam.Key] = queryParam.Value;
                }

                var stringUri = uri.ToString();

                if (stringUri.Contains("?"))
                {
                    stringUri += $"&{queryString}";
                }
                else
                {
                    stringUri += $"?{queryString}";
                }

                return new Uri(stringUri, UriKind.Relative);
            }
        }
    }


    public class JsonContent : StringContent
    {
        public JsonContent(object value)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8,
                MimeTypes.Application.Json)
        {
        }

        public JsonContent(object value, string mediaType)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8, mediaType)
        {
        }
    }

    public class PatchContent : StringContent
    {
        public PatchContent(object value)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8,
                MimeTypes.Application.JsonPatch)
        {
        }
    }

    public class FileContent : MultipartFormDataContent
    {
        public FileContent(string filePath, string apiParamName)
        {
            var fileStream = File.Open(filePath, FileMode.Open);
            var filename = Path.GetFileName(filePath);

            Add(new StreamContent(fileStream), apiParamName, filename);
        }
    }
}