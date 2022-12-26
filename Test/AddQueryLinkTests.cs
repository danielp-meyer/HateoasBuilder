using System;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddQueryLinkTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpClient.AddQueryLink (pass).")]
        [InlineData("https://foo.bar/relative1?relative=ball", "relative1", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/relative2/ball/dingle", "relative2", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/relative3?relative2=ball&dingle=2", "relative3", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddQueryLinkHttpContextPass(string result, string relativeUrl, object[] items)
        {
            var links = GetHttpContext().AddQueryLink(rel, relativeUrl, items).Build();

            Assert.Equal(new Link(rel, result), links.First());
        }

        [Theory(DisplayName = "String.AddQueryLink (pass).")]
        [InlineData("https://foo.bar/relative/ball", "relative1", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/relative1/ball/dingle", "relative2", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/relative2/ball/dingle/2", "relative3", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddQueryLinkStringPass(string result, string relativeUrl, object[] items)
        {
            var links = baseUrl.AddQueryLink(rel, relativeUrl, items).Build();

            Assert.Equal(new Link(rel,result), links.First());
        }

        [Theory(DisplayName = "HttpContext.AddQueryLink (fail).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", "\t", new object[] { "ball", "dingle", 2 })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "relative1", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddQueryLinkHttpContextFail(string result, string? rel, string baseUrl, object[] items)
        {
            var caught = Assert.Throws<ArgumentException>(() => GetHttpContext().AddQueryLink(rel, baseUrl, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "HttpContext.AddQueryLink (fail2).")]
        [InlineData("Value cannot be null. (Parameter 'routeItems')", rel, baseUrl, null)]
        public void AddQueryLinkHttpContextFail2(string result, string? rel, string baseUrl, object[] items)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => GetHttpContext().AddQueryLink(rel, baseUrl, items));

            Assert.Equal(result, caught.Message);
        }
    }
}