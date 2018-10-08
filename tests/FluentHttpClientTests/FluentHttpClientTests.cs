using System.Net.Http;
using FluentHttpClient;
using Xunit;

namespace FluentHttpClientTests
{
    public class FluentHttpClientTests
    {
        [Fact]
        public void WithBaseUrl_SetBaseUrl_ClientShouldHaveBaseUrl()
        {
            //const string baseUrl = "Http://Chehre.net/";

            //var fluentHttpClient = FluentHttpClient.FluentHttpClient.NewFluentHttpClient().WithBaseUrl(baseUrl).Build();


            //Assert.Equal(baseUrl.ToLower(), fluentHttpClient.RawHttpClient.BaseAddress.AbsoluteUri.ToLower());


            var client = FluentHttpClient.FluentHttpClient.NewFluentHttpClient().WithTimeout(500)
                .UseMiddleware<NlogFluentHttpClientMiddleware>().UseMiddleware<Nlog2FluentHttpClientMiddleware>().Build();
            var a = client.PostAsJson<string>("", new{Id=1 });


            //client.SendAsync(request).Result.;
        }


        [Fact]
        public void WithBaseUrl_SetTimeout_ClientShouldHaveTimeout()
        {
            //const int timeout = 10;

            //var fluentHttpClient = FluentHttpClient.FluentHttpClient.NewFluentHttpClient().WithTimeout(10).Build();


            //Assert.Equal(timeout, fluentHttpClient.RawHttpClient.Timeout.TotalSeconds);
        }
    }
}