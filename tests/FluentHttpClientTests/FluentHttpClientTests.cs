using System.Collections.Generic;
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
            var client = FluentHttpClient.FluentHttpClient.NewFluentHttpClient().WithTimeout(500).WithBaseUrl("http://172.25.25.4:7001/api/")
                .UseMiddleware<NlogFluentHttpClientMiddleware>().UseMiddleware<Nlog2FluentHttpClientMiddleware>().Build();
            var a = client.Get<List<Event>>("events");
        }


        [Fact]
        public void WithBaseUrl_SetTimeout_ClientShouldHaveTimeout()
        {
           
        }
    }



    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
    }
}