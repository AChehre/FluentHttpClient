using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentHttpClient;
using FluentHttpClient.Client;
using Moq;
using Xunit;

namespace FluentHttpClientTests
{
    public class FluentHttpClientTests
    {
        [Fact]
        public async Task GetAsyncAsString_return_valid_StatusCode_and_Content()
        {
            const string stringContent = "found";
            const string baseUrl = "http://chehre.net/api/";

            var fakeMessageHandler = new Mock<FakeHttpMessageHandler>
            {
                CallBase = true
            };


            fakeMessageHandler.Setup(t => t.Send(It.Is<HttpRequestMessage>(
                    msg =>
                        msg.Method == HttpMethod.Get &&
                        msg.RequestUri.ToString() == baseUrl)))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(stringContent)
                });


            var client = FluentHttpClient.FluentHttpClient.NewFluentHttpClient()
                .WithHttpMessageHandler(fakeMessageHandler.Object)
                .WithBaseUrl(baseUrl).Build();


            var response = await client.GetAsync("");
            var content = await response.AsString();


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(stringContent, content);
        }


        [Fact]
        public void SetBearerToken_RawClient_should_have_bearer_key()
        {
            const string token = "my token";
            const string baseUrl = "http://chehre.net/api/";

            var client = FluentHttpClient.FluentHttpClient.NewFluentHttpClient().ConfigDefaultRequestHeaders(
                    defaultRequestHeaders => { defaultRequestHeaders.SetBearerToken(token); })
                .WithBaseUrl(baseUrl).Build();


            Assert.Equal("Bearer".ToLower(), client.RawHttpClient.DefaultRequestHeaders.Authorization.Scheme.ToLower());
            Assert.Equal(token, client.RawHttpClient.DefaultRequestHeaders.Authorization.Parameter.ToLower());
        }
    }
}