using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentHttpClient
{
    public class FluentHttpClient : IDisposable
    {
        private readonly IList<string> _acceptHeaders;
        private readonly string _baseUrl;
        private readonly IList<FluentHttpClientMiddleware> _fluentHttpClientMiddlewares = new List<FluentHttpClientMiddleware>();


        private FluentHttpClient(IHttpClientBuilder httpClientBuilder)
        {
            _baseUrl = httpClientBuilder.BaseUrl;
            _acceptHeaders = httpClientBuilder.AcceptHeaders;
        }

        public HttpClient RawHttpClient { get; private set; }

        public void Dispose()
        {
            RawHttpClient?.Dispose();
        }


        public static HttpClientBuilder NewFluentHttpClient()
        {
            return new HttpClientBuilder();
        }


        public T GetAsJson<T>(string uri)
        {
            var response = RawHttpClient.GetAsync(new Uri($"{_baseUrl}/{uri}")).Result;
            var stringResult = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(stringResult);
        }


        public async Task<T> GetAsJsonAsync<T>(string uri)
        {
            var response = await RawHttpClient.GetAsync(new Uri($"{_baseUrl}/{uri}"));
            var stringResult = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(stringResult);
        }


        public HttpResponseMessage PostAsJson(string uri, object data)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

            return RawHttpClient.PostAsync(new Uri($"{_baseUrl}/{uri}"), content).Result;
        }


        public async Task<HttpResponseMessage> PostAsJsonAsync(string uri, object data)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");

            return await RawHttpClient.PostAsync(new Uri($"{_baseUrl}/{uri}"), content);
        }


        private interface IHttpClientBuilder
        {
            string BaseUrl { get; }
            IList<string> AcceptHeaders { get; }
            int Timeout { get; }
            IList<FluentHttpClientMiddleware> FluentHttpClientMiddlewares { get; }
        }


        public class HttpClientBuilder : IHttpClientBuilder
        {
            private readonly IList<string> _acceptHeaders = new List<string>();
            private string _baseUrl;
            private int _timeout;
            private readonly IList<FluentHttpClientMiddleware> _fluentHttpClientMiddlewares = new List<FluentHttpClientMiddleware>();

            string IHttpClientBuilder.BaseUrl => _baseUrl;


            IList<string> IHttpClientBuilder.AcceptHeaders => _acceptHeaders;
            IList<FluentHttpClientMiddleware> IHttpClientBuilder.FluentHttpClientMiddlewares  => _fluentHttpClientMiddlewares;

            int IHttpClientBuilder.Timeout => _timeout;


            public HttpClientBuilder AddAcceptHeader(string value)
            {
                _acceptHeaders.Add(value);
                return this;
            }

            public HttpClientBuilder AddMiddleware(FluentHttpClientMiddleware middleware)
            {
                _fluentHttpClientMiddlewares.Add(middleware);
                return this;
            }


            public HttpClientBuilder AddApplicationJsonHeader()
            {
                return AddAcceptHeader("application/json");
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

                foreach (var acceptHeader in _acceptHeaders)
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
                }


                return httpClient;
            }
        }
    }
}