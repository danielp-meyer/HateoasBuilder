using System;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddRouteLinkTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpClient.AddRouteLink (pass).")]
        [InlineData("https://foo.bar/relative/ball", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/relative1/ball/dingle", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/relative2/ball/dingle/2", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddRouteLinkHttpContextPass(string result, object[] items)
        {
            var links = GetHttpContext().AddRouteLink(rel, items).Build();

            Assert.Equal(new Link(rel,result), links.First());
        }

        [Theory(DisplayName = "String.AddRouteLink (pass).")]
        [InlineData("https://foo.bar/relative/ball", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/relative1/ball/dingle", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/relative2/ball/dingle/2", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddRouteLinkStringPass(string result, object[] items)
        {
            var links = baseUrl.AddRouteLink(rel, items).Build();

            Assert.Equal(new Link(rel,result), links.First());
        }

        [Theory(DisplayName = "HttpContext.AddRouteLink (fail).")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddRouteLinkHttpContextFail(string result, string? rel, object[] items)
        {
            var caught = Assert.Throws<ArgumentException>(() => GetHttpContext().AddRouteLink(rel, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "HttpContext.AddRouteLink (fail2).")]
        [InlineData("Value cannot be null. (Parameter 'routeItems')", rel, null)]
        public void AddRouteLinkHttpContextFail2(string result, string? rel, object[] items)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => GetHttpContext().AddRouteLink(rel, items));

            Assert.Equal(result, caught.Message);
        }
    }
}