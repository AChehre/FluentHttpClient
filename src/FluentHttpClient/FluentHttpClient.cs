using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FluentHttpClient
{
    public class FluentHttpClient
    {
        private readonly string _baseUrl;
        private readonly Dictionary<string, string> _headers;


        private FluentHttpClient(IHttpClientBuilder httpClientBuilder)
        {
            _baseUrl = httpClientBuilder.BaseUrl;
            _headers = httpClientBuilder.Headers;
        }

        public HttpClient RawHttpClient { get; private set; }


        public static HttpClientBuilder NewFluentHttpClient()
        {
            return new HttpClientBuilder();
        }


        private interface IHttpClientBuilder
        {
            string BaseUrl { get; }
            Dictionary<string, string> Headers { get; }
            int Timeout { get; }
        }


        public class HttpClientBuilder : IHttpClientBuilder
        {
            private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
            private string _baseUrl;
            private int _timeout;


            string IHttpClientBuilder.BaseUrl => _baseUrl;


            Dictionary<string, string> IHttpClientBuilder.Headers => _headers;

            int IHttpClientBuilder.Timeout => _timeout;


            public HttpClientBuilder AddHeader(string key, string value)
            {
                _headers[key] = value;
                return this;
            }

            public HttpClientBuilder AddApplicationJsonHeader()
            {
                _headers["content-type"] = "application/json";
                return this;
            }


            public HttpClientBuilder WithBaseUrl(string baseUrl)
            {
                _baseUrl = baseUrl;
                return this;
            }


            public HttpClientBuilder WithTimeout(int timeout)
            {
                _timeout = timeout;
                return this;
            }


            public FluentHttpClient Build()
            {
                var fluentHttpClient = new FluentHttpClient(this);
                fluentHttpClient.RawHttpClient = BuildHttpClient(this);

                return fluentHttpClient;
            }


            private HttpClient BuildHttpClient(IHttpClientBuilder httpClientBuilder)
            {
                var httpClient = new HttpClient();

                if (!string.IsNullOrWhiteSpace(httpClientBuilder.BaseUrl))
                {
                    httpClient.BaseAddress = new Uri(httpClientBuilder.BaseUrl);
                }

                if (httpClientBuilder.Timeout > 0)

                {
                    httpClient.Timeout = new TimeSpan(0, 0, httpClientBuilder.Timeout);
                }

                foreach (var header in httpClientBuilder.Headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                return httpClient;
            }
        }
    }
}