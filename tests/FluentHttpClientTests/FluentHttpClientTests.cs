using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentHttpClient;
using Moq;
using Xunit;

namespace FluentHttpClientTests
{
    public class FluentHttpClientTests
    {
        [Fact]
        public async Task WithBaseUrl_SetBaseUrl_ClientShouldHaveBaseUrl()
        {
            const string stringContent = "found";


            var fakeMessageHandler = new Mock<FakeHttpMessageHandler>
            {
                CallBase = true
            };


            fakeMessageHandler.Setup(t => t.Send(It.Is<HttpRequestMessage>(
                    msg =>
                        msg.Method == HttpMethod.Get &&
                        msg.RequestUri.ToString() == "http://chehre.net/api/")))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(stringContent)
                });


            var client = FluentHttpClient.FluentHttpClient.NewFluentHttpClient()
                .WithHttpMessageHandler(fakeMessageHandler.Object)
                .WithBaseUrl("http://chehre.net/api/").Build();


            var response = await client.GetAsync("");
            var content = await response.AsString();


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(stringContent, content);
        }


        [Fact]
        public void WithBaseUrl_SetTimeout_ClientShouldHaveTimeout()
        {
        }
    }


    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException("Remember to setup this method with your mocking framework");
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}