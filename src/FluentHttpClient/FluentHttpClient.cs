using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentHttpClient
{
    public class FluentHttpClient : IDisposable
    {
        private readonly FluentHttpClientRequestDelegate _requestDelegate;


        private FluentHttpClient(IFluentHttpClientBuilder httpClientBuilder)
        {
            _requestDelegate = httpClientBuilder.RequestDelegate;
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


        private async Task<FluentHttpClientResponse> RawHttpClientSendAsync(FluentHttpClientRequest request,
            CancellationToken cancellationToken)
        {
            var response = await RawHttpClient
                .SendAsync(request.Message, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            return new FluentHttpClientResponse(response);
        }

        public async Task<FluentHttpClientResponse<T>> SendAsync<T>(FluentHttpClientRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FluentHttpClientResponse response;

            if (_requestDelegate != null)
            {
                response = await _requestDelegate.Invoke(request, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                response = await RawHttpClientSendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            if (request.EnsureSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }

            if (!response.IsSuccessStatusCode)
            {
                return new FluentHttpClientResponse<T>(response);
            }


            return new FluentHttpClientResponse<T>(response)
            {
                Content = await response.As<T>()
            };
        }


        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default(T);
            }

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }

        public async Task<FluentHttpClientResponse> SendAsync(FluentHttpClientRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_requestDelegate != null)
            {
                return await _requestDelegate.Invoke(request, cancellationToken).ConfigureAwait(false);
            }

            return await RawHttpClientSendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private interface IFluentHttpClientBuilder
        {
            Uri BaseUrl { get; }
            IList<string> AcceptHeaders { get; }
            int Timeout { get; }
            FluentHttpClientRequest Request { get; }
            HttpMessageHandler HttpMessageHandler { get; }
            IList<FluentHttpClientMiddlewareConfig> Middlewares { get; }
            FluentHttpClientRequestDelegate RequestDelegate { get; }
            Action<HttpRequestHeaders> DefaultRequestHeaders { get; }
        }


        public class FluentHttpClientBuilder : IFluentHttpClientBuilder
        {
            private readonly IList<string> _acceptHeaders = new List<string>();


            private readonly IList<FluentHttpClientMiddlewareConfig> _middlewares =
                new List<FluentHttpClientMiddlewareConfig>();


            private Uri _baseUrl;

            private Action<HttpRequestHeaders> _defaultRequestHeaders;

            private HttpMessageHandler _httpMessageHandler;


            private FluentHttpClientRequestDelegate _requestDelegate;


            private int _timeout;


            FluentHttpClientRequestDelegate IFluentHttpClientBuilder.RequestDelegate => _requestDelegate;

            HttpMessageHandler IFluentHttpClientBuilder.HttpMessageHandler => _httpMessageHandler;
            Uri IFluentHttpClientBuilder.BaseUrl => _baseUrl;


            FluentHttpClientRequest IFluentHttpClientBuilder.Request { get; }
            IList<string> IFluentHttpClientBuilder.AcceptHeaders => _acceptHeaders;
            IList<FluentHttpClientMiddlewareConfig> IFluentHttpClientBuilder.Middlewares => _middlewares;
            int IFluentHttpClientBuilder.Timeout => _timeout;

            Action<HttpRequestHeaders> IFluentHttpClientBuilder.DefaultRequestHeaders => _defaultRequestHeaders;


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
                return AddAcceptHeader(MimeTypes.Application.Json);
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


            public FluentHttpClientBuilder ConfigDefaultRequestHeaders(Action<HttpRequestHeaders> defaultRequestHeaders)
            {
                _defaultRequestHeaders = defaultRequestHeaders;

                return this;
            }


            public FluentHttpClientBuilder WithHttpMessageHandler(HttpMessageHandler handler)
            {
                _httpMessageHandler = handler;
                return this;
            }


            public FluentHttpClient Build()
            {
                if (_baseUrl == null)
                {
                    _baseUrl = new Uri("");
                }


                var fluentHttpClient = new FluentHttpClient(this)
                {
                    RawHttpClient = BuildHttpClient(this)
                };


                _defaultRequestHeaders?.Invoke(fluentHttpClient.RawHttpClient.DefaultRequestHeaders);


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