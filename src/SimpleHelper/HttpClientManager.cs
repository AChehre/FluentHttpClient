using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using NLog;

namespace SimpleHelper
{
    public class HttpClientManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _httpClient;



        




        public HttpClientManager(string baseUrl)
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(baseUrl)};
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClientManager(string baseUrl, int timeout,
            ConcurrentDictionary<RequestHeaderItems.HeaderType, RequestHeaderItems> requestHeaderItems = null)
        {
            _httpClient = new HttpClient {BaseAddress = new Uri(baseUrl), Timeout = new TimeSpan(0, 0, timeout)};
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            if (requestHeaderItems != null)
            {
                foreach (var item in requestHeaderItems)
                {
                    var requestHeaderItem = item.Value;
                    if (item.Key == RequestHeaderItems.HeaderType.Authorization)
                    {
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(requestHeaderItem.Key, requestHeaderItem.Value);
                    }
                    else
                    {
                        _httpClient.DefaultRequestHeaders.Add(requestHeaderItem.Key, requestHeaderItem.Value);
                    }
                }
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public T Execute<T>(HttpMethod verb, string serviceName, object param)
        {
            var response = GetResponse(verb, serviceName, param);
            if (typeof(T) == typeof(string))
            {
                var stringResult = response.Content.ReadAsStringAsync().Result;
                var result = (T) Convert.ChangeType(stringResult, typeof(T));
                return result;
            }

            return response.Content.ReadAsAsync<T>().Result;
        }

        private HttpResponseMessage GetResponse(HttpMethod verb, string serviceName, object param)
        {
            HttpResponseMessage response;

            if (verb == HttpMethod.Post)
            {
                response = _httpClient.PostAsJsonAsync(serviceName, param).Result;
                Logger.Info($"{response.StatusCode}, {response.Content.ReadAsStringAsync()}");
            }
            else if (verb == HttpMethod.Put)
            {
                response = _httpClient.PutAsJsonAsync(serviceName, param).Result;
            }
            else if (verb == HttpMethod.Delete)
            {
                response = _httpClient.DeleteAsync(serviceName).Result;
            }
            else
            {
                if (param != null)
                {
                    serviceName += "?" + param;
                }

                response = _httpClient.GetAsync(serviceName).Result;
            }

            return response.EnsureSuccessStatusCode();
        }
    }
}