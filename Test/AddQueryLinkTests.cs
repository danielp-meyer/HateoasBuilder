using System;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddQueryLinkTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpContext.AddQueryLink (pass).")]
        [InlineData("https://foo.bar/?relative=ball", "relative1", null, new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/?relative1=ball&dingle=", "relative2", "", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/?relative2=ball&dingle=2", "relative3", "\t", new object[] { "relative2", "ball", "dingle", 2 })]
        [InlineData("https://foo.bar/baseUrl?relative=ball", "relative1", "baseUrl", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/baseUrl1?relative1=ball&dingle=", "relative2", "baseUrl1", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/baseUrl2?relative2=ball&dingle=2", "relative3", "baseUrl2", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddQueryLinkHttpContextPass(string result, string relLabel, string relativeBaseUrl, object[] items)
        {
            var links = GetHttpContext().AddQueryLink(relLabel, relativeBaseUrl, items).Build();

            Assert.Equal(new Link(relLabel, result), links.First());
        }

        [Theory(DisplayName = "String.AddQueryLink (pass).")]
        [InlineData("https://foo.bar/?relative=ball", "relative1", null, new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/?relative1=ball&dingle=", "relative2", "", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/?relative2=ball&dingle=2", "relative3", "\t", new object[] { "relative2", "ball", "dingle", 2 })]
        [InlineData("https://foo.bar/baseUrl?relative=ball", "relative1", "baseUrl", new object[] { "relative", "ball" })]
        [InlineData("https://foo.bar/baseUrl1?relative1=ball&dingle=", "relative2", "baseUrl1", new object[] { "relative1", "ball", "dingle" })]
        [InlineData("https://foo.bar/baseUrl2?relative2=ball&dingle=2", "relative3", "baseUrl2", new object[] { "relative2", "ball", "dingle", 2 })]
        public void AddQueryLinkStringPass(string result, string relLabel, string relativeBaseUrl, object[] items)
        {
            var links = baseUrl.AddQueryLink(relLabel, relativeBaseUrl, items).Build();

            Assert.Equal(new Link(relLabel, result), links.First());
        }

        [Theory(DisplayName = "HttpContext.AddQueryLink (fail).")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", null, null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relLabel')", "\t", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddQueryLinkHttpContextFail(string result, string relLabel, string relativeBaseUrl, object[] items)
        {
            var caught = Assert.Throws<ArgumentException>(() => GetHttpContext().AddQueryLink(relLabel, relativeBaseUrl, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "HttpContext.AddQueryLink (fail2).")]
        [InlineData("Value cannot be null. (Parameter 'queryPairs')", rel, baseUrl, null)]
        public void AddQueryLinkHttpContextFail2(string result, string? relLabel, string baseUrl, object[] items)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => GetHttpContext().AddQueryLink(relLabel, baseUrl, items));

            Assert.Equal(result, caught.Message);
        }
    }
}