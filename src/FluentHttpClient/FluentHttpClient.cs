using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FluentHttpClient
{
    public class FluentHttpClient : IDisposable
    {
        private readonly FluentHttpClientRequestDelegate _requestDelegate;
        public FluentFormatterOption FormatterOption;


        private FluentHttpClient(IFluentHttpClientBuilder httpClientBuilder)
        {
            FormatterOption = httpClientBuilder.FormatterOption;
            _requestDelegate = httpClientBuilder.RequestDelegate;
        }

        public FluentFormatterOption FluentFormatterOption => FormatterOption;

        public HttpClient RawHttpClient { get; private set; }

        public void Dispose()
        {
            RawHttpClient?.Dispose();
        }


        public static FluentHttpClientBuilder NewFluentHttpClient()
        {
            return new FluentHttpClientBuilder();
        }


        private async Task<FluentHttpClientResponse> RawHttpClientSendAsync(FluentHttpClientRequest request)
        {
            var response = await RawHttpClient.SendAsync(request.Message).ConfigureAwait(false);
            return new FluentHttpClientResponse(response);
        }

        public async Task<FluentHttpClientResponse<T>> SendAsync<T>(FluentHttpClientRequest request)
        {
            FluentHttpClientResponse response;

            if (_requestDelegate != null)
            {
                response = await _requestDelegate.Invoke(request).ConfigureAwait(false);
            }
            else
            {
                response = await RawHttpClientSendAsync(request).ConfigureAwait(false);
            }


            return new FluentHttpClientResponse<T>(response)
            {
                Content = await response.Message.Content.ReadAsAsync<T>().ConfigureAwait(false)
            };
        }


        public async Task<FluentHttpClientResponse> SendAsync(FluentHttpClientRequest request)
        {
            if (_requestDelegate != null)
            {
                return await _requestDelegate.Invoke(request).ConfigureAwait(false);
            }

            return await RawHttpClientSendAsync(request).ConfigureAwait(false);
        }

        private interface IFluentHttpClientBuilder
        {
            Uri BaseUrl { get; }
            IList<string> AcceptHeaders { get; }
            int Timeout { get; }
            FluentFormatterOption FormatterOption { get; }
            FluentHttpClientRequest Request { get; }
            HttpMessageHandler HttpMessageHandler { get; }
            IList<FluentHttpClientMiddlewareConfig> Middlewares { get; }
            FluentHttpClientRequestDelegate RequestDelegate { get; }
        }


        public class FluentHttpClientBuilder : IFluentHttpClientBuilder
        {
            private readonly IList<string> _acceptHeaders = new List<string>();

            private readonly FluentFormatterOption _formatterOption = new FluentFormatterOption();



            private readonly IList<FluentHttpClientMiddlewareConfig> _middlewares =
                new List<FluentHttpClientMiddlewareConfig>();


            private Uri _baseUrl;

            private HttpMessageHandler _httpMessageHandler;


            private FluentHttpClientRequestDelegate _requestDelegate;


            private int _timeout;


            FluentHttpClientRequestDelegate IFluentHttpClientBuilder.RequestDelegate => _requestDelegate;

            HttpMessageHandler IFluentHttpClientBuilder.HttpMessageHandler => _httpMessageHandler;
            Uri IFluentHttpClientBuilder.BaseUrl => _baseUrl;
            FluentFormatterOption IFluentHttpClientBuilder.FormatterOption => _formatterOption;


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
                _baseUrl = new Uri(baseUrl);
                return this;
            }

            public FluentHttpClientBuilder WithBaseUrl(Uri baseUrl)
            {
                _baseUrl = baseUrl;
                return this;
            }


            public FluentHttpClientBuilder WithTimeout(int timeout)
            {
                _timeout = timeout;
                return this;
            }

            public FluentHttpClientBuilder ConfigFormatter(Action<FluentFormatterOption> config)
            {
                config(_formatterOption);
                return this;
            }


            public FluentHttpClientBuilder WithHttpMessageHandler(HttpMessageHandler handler)
            {
                _httpMessageHandler = handler;
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

                FluentHttpClientRequestDelegate invokeDelegate = fluentHttpClient.RawHttpClientSendAsync;

                _requestDelegate = CreateMiddlewares(invokeDelegate);
                ;


                return fluentHttpClient;
            }

            private FluentHttpClientRequestDelegate CreateMiddlewares(FluentHttpClientRequestDelegate invokeDelegate)
            {
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

                return invokeDelegate;
            }


        


            private HttpClient BuildHttpClient(IFluentHttpClientBuilder httpClientBuilder)
            {
                var httpClient = httpClientBuilder.HttpMessageHandler != null
                    ? new HttpClient(httpClientBuilder.HttpMessageHandler)
                    : new HttpClient();

                httpClient.BaseAddress = httpClientBuilder.BaseUrl;

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