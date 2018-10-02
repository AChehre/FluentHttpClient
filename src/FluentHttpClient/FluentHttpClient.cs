using System;
using System.Collections.Generic;
using System.Linq;
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
        private FluentHttpClientRequestDelegate _requestDelegate;


        private FluentHttpClient(IFluentHttpClientBuilder httpClientBuilder)
        {
            _baseUrl = httpClientBuilder.BaseUrl;
            _acceptHeaders = httpClientBuilder.AcceptHeaders;
        }

        public HttpClient RawHttpClient { get; private set; }

        public void Dispose()
        {
            RawHttpClient?.Dispose();
        }


        public static FluentHttpClientBuilder NewFluentHttpClient()
        {
            return new FluentHttpClientBuilder();
        }


        private async Task<FluentHttpClientResponse> SendMessageAsync(FluentHttpClientRequest message)
        {
            var response = await RawHttpClient.SendAsync(message.Message);
            return new FluentHttpClientResponse(response);
        }


        public async Task<FluentHttpClientResponse> SendAsync(FluentHttpClientRequest message)
        {
            if (_requestDelegate != null)
            {
                return await _requestDelegate.Invoke(message);
            }

            return await SendMessageAsync(message);
        }


        private interface IFluentHttpClientBuilder
        {
            string BaseUrl { get; }
            IList<string> AcceptHeaders { get; }
            int Timeout { get; }

            FluentHttpClientRequest Request { get; }
            IList<FluentHttpClientMiddlewareConfig> Middlewares { get; }
        }


        public class FluentHttpClientBuilder : IFluentHttpClientBuilder
        {
            private readonly IList<string> _acceptHeaders = new List<string>();


            private readonly IList<FluentHttpClientMiddlewareConfig> _middlewares =
                new List<FluentHttpClientMiddlewareConfig>();


            private string _baseUrl;

            private int _timeout;
            string IFluentHttpClientBuilder.BaseUrl => _baseUrl;


            FluentHttpClientRequest IFluentHttpClientBuilder.Request { get; }

            IList<string> IFluentHttpClientBuilder.AcceptHeaders => _acceptHeaders;
            IList<FluentHttpClientMiddlewareConfig> IFluentHttpClientBuilder.Middlewares => _middlewares;

            int IFluentHttpClientBuilder.Timeout => _timeout;


            public FluentHttpClientBuilder AddAcceptHeader(string value)
            {
                _acceptHeaders.Add(value);
                return this;
            }

            public FluentHttpClientBuilder UseMiddleware<T>(params object[] args) where T : IFluentHttpClientMiddleware
            {
                return UseMiddleware(typeof(T), args);
            }


            public FluentHttpClientBuilder UseMiddleware(Type middleware, params object[] args)
            {
                _middlewares.Add(new FluentHttpClientMiddlewareConfig(middleware, args));
                return this;
            }


            public FluentHttpClientBuilder AddApplicationJsonHeader()
            {
                return AddAcceptHeader("application/json");
            }


            public FluentHttpClientBuilder WithBaseUrl(string baseUrl)
            {
                _baseUrl = baseUrl;
                return this;
            }


            public FluentHttpClientBuilder WithTimeout(int timeout)
            {
                _timeout = timeout;
                return this;
            }


            public FluentHttpClient Build()
            {
                var fluentHttpClient = new FluentHttpClient(this)
                {
                    RawHttpClient = BuildHttpClient(this)
                };


                if (_middlewares == null || !_middlewares.Any())
                {
                    return fluentHttpClient;
                }

                FluentHttpClientRequestDelegate invokeDelegate = fluentHttpClient.SendMessageAsync;


                foreach (var middlewareConfig in _middlewares)
                {
                    object[] constructorArgs;
                    if (middlewareConfig.Args == null)
                    {
                        constructorArgs = new object[] {invokeDelegate};
                    }
                    else
                    {
                        constructorArgs = new object[middlewareConfig.Args.Length + 1];
                        constructorArgs[0] = invokeDelegate;
                        Array.Copy(middlewareConfig.Args, 0, constructorArgs, 1, middlewareConfig.Args.Length);
                    }

                    var middleware = (IFluentHttpClientMiddleware) Activator.CreateInstance(middlewareConfig.Middleware,
                        constructorArgs);
                    invokeDelegate = middleware.InvokeAsync;
                }


                fluentHttpClient._requestDelegate = invokeDelegate;


                return fluentHttpClient;
            }


            private HttpClient BuildHttpClient(IFluentHttpClientBuilder httpClientBuilder)
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


        #region Json Reques

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

        #endregion
    }
}