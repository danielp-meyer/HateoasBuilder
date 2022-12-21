using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class ExtensionTest
    {
        [Theory(DisplayName = "AddLink (fail).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball" , "dingle", 2})]
        public void AddLinkTest(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var http = new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });

            var linkbuilder = http.AddLink(relLabel, relPathFormat, items);

            Assert.Equal(new Link
            {
                Href = result,
                Rel = relLabel,
            }, linkbuilder.GetLinks().First());
        }
    }
}