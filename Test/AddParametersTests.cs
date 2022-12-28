using System;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddParametersTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpContext.AddLink (pass)")]
        [InlineData(true)]
        [InlineData(false)]
        public void AddParametersToAddLinkTrueTest(bool condition)
        {
            var links = GetHttpContext()
                .AddLink(condition, rel, "relativeUrl")
                .AddParameters("name", "value")
                .AddLink(!condition, rel, "relativeUrl")
                .AddParameters("name", "value")
                .Build();

            Assert.Single(links);
            Assert.Equal("https://foo.bar/relativeUrl?name=value", links.First().Href);
            Assert.Equal(rel, links.First().Rel);
        }

        [Theory(DisplayName = "HttpContext.AddLink (pass)")]
        [InlineData(true)]
        [InlineData(false)]
        public void AddParametersToAddRouteLinkTrueTest(bool condition)
        {
            var links = GetHttpContext()
                .AddRouteLink(condition, rel, "relativeUrl")
                .AddParameters("name", "value")
                .AddRouteLink(!condition, rel, "relativeUrl")
                .AddParameters("name", "value")
                .Build();

            Assert.Single(links);
            Assert.Equal("https://foo.bar/relativeUrl?name=value", links.First().Href);
            Assert.Equal(rel, links.First().Rel);
        }

        [Theory(DisplayName = "HttpContext.AddQueryLink (pass)")]
        [InlineData(true)]
        [InlineData(false)]
        public void AddParametersToAddQueryLinkTrueTest(bool condition)
        {
            var links = GetHttpContext()
                .AddQueryLink(condition, rel, "relativeUrl", "name1", "value1")
                .AddParameters("name", "value")
                .AddQueryLink(!condition, rel, "relativeUrl", "name1", "value1")
                .AddParameters("name", "value")
                .Build();

            Assert.Single(links);
            Assert.Equal("https://foo.bar/relativeUrl?name1=value1&name=value", links.First().Href);
            Assert.Equal(rel, links.First().Rel);
        }

        [Theory(DisplayName = "HttpContext.AddLink (fail)")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", null, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "", "dingle")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "\t", "dingle")]
        public void AddLinkHttpContextFailTest(string result, string rel, string relativeUrl)
        {
            var ex = Assert.Throws<ArgumentException>(() => GetHttpContext().AddLink(rel, relativeUrl));

            Assert.Equal(result, ex.Message);
        }

        [Theory(DisplayName = "String.AddLink (fail)")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'baseUrl')", null, rel, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'baseUrl')", "", rel, "dingle")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'baseUrl')", "\t", rel, "dingle")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", baseUrl, null, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", baseUrl, "", "dingle")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", baseUrl, "\t", "dingle")]
        public void AddLinkStringFailTest(string result, string baseUrl, string label, string relativeUrl)
        {
            var ex = Assert.ThrowsAny<ArgumentException>(() => baseUrl.AddLink(label, relativeUrl));

            Assert.Equal(result, ex.Message);
        }
    }
}