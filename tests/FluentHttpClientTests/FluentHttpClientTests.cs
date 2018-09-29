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