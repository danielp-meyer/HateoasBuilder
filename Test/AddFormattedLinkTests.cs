using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddFormattedLinkTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpClient.AddFormattedLink (pass).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkHttpContextPass(string result, string relPathFormat, object[] items)
        {
            var linkbuilder = GetHttpContext()
                .AddFormattedLink(rel, relPathFormat, items)
                .Build()
                .First();

            var expected = new Link(rel, result);

            Assert.Equal(expected, linkbuilder);
        }

        [Theory(DisplayName = "HttpContext.AddFormattedLink (fail).")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkHttpContextFail(string result, string? relPathFormat, object[] items)
        {
            var caught = Assert.Throws<ArgumentException>(() => GetHttpContext().AddFormattedLink(rel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "String.AddFormattedLink (pass).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkStringPass(string result, string relPathFormat, object[] items)
        {
            var linkbuilder = baseUrl.AddFormattedLink(rel, relPathFormat, items);

            Assert.Equal(new Link(rel,result), linkbuilder.Build().First());
        }

        [Theory(DisplayName = "String.AddFormattedLink (fail).")]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty, or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkStringFail(string result, string? relPathFormat, object[] items)
        {
            var caught = Assert.Throws<ArgumentException>(() => GetHttpContext().AddFormattedLink(rel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }
    }
}