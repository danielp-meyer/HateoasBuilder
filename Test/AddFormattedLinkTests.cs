using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddFormattedLinkTests:ExtensionTest
    {
        [Theory(DisplayName = "HttpClient.AddLink (pass).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkHttpContextPass(string result, string relPathFormat, object[] items)
        {
            var linkbuilder = GetHttpContext().AddFormattedLink(rel, relPathFormat, items);

            Assert.Equal(new Link
            {
                Href = result,
                Rel = rel,
            }, linkbuilder.Build().First());
        }

        [Theory(DisplayName = "HttpContext.AddLink (fail).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkHttpContextFail(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var http = new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });

            var caught = Assert.Throws<ArgumentException>(() => http.AddFormattedLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "String.AddLink (pass).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkStringPass(string result, string relPathFormat, object[] items)
        {
            var linkbuilder = baseUrl.AddFormattedLink(rel, relPathFormat, items);

            Assert.Equal(new Link
            {
                Href = result,
                Rel = rel,
            }, linkbuilder.Build().First());
        }

        [Theory(DisplayName = "String.AddLink (fail).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddFormattedLinkStringFail(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var http = new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });

            var caught = Assert.Throws<ArgumentException>(() => http.AddFormattedLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }


        // [Theory(DisplayName = "BaseUrl.AddLink (pass).")]
        // [InlineData("https://foo.bar/Dingle", "Dingle", new object[] { })]
        // [InlineData("https://foo.bar/DingleBall", "Dingle{0}", new object[] { "Ball" })]
        // [InlineData("https://foo.bar/DingleBalldingle", "Dingle{0}{1}", new object[] { "Ball", "dingle" })]
        // [InlineData("https://foo.bar/DingleBalldingle2", "Dingle{0}{1}{2}", new object[] { "Ball", "dingle", 2 })]
        // public void AddLinkTest5(string result, string? relPathFormat, object[] items)
        // {
        //     const string relLabel = "rel";
        //     const string baseUrl = "https://foo.bar";

        //     var output = baseUrl.AddFormattedLink(relLabel, relPathFormat, items).Build().First();
        //     var expected = new Link
        //     {
        //         Href = result,
        //         Rel = "rel"
        //     };

        //     Assert.Equal(expected.Href, output.Href);
        //     Assert.Equal(expected.Rel, output.Rel);
        // }

        // [Theory(DisplayName = "BaseUrl.AddLink (fail 1).")]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "", "relPathFormat", new object[] { "ball" })]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", null, "relPathFormat", new object[] { "ball", "dingle" })]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "\t", "relPathFormat", new object[] { "ball", "dingle", 2 })]
        // public void AddLinkTest6(string result, string? baseUrl, string? relPathFormat, object[] items)
        // {
        //     const string relLabel = "rel";

        //     var caught = Assert.Throws<ArgumentException>(() => baseUrl.AddFormattedLink(relLabel, relPathFormat, items));

        //     Assert.Equal(result, caught.Message);
        // }

        // [Theory(DisplayName = "BaseUrl.AddLink (fail 2).")]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "", new object[] { "ball" })]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", null, new object[] { "ball", "dingle" })]
        // [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "\t", new object[] { "ball", "dingle", 2 })]
        // public void AddLinkTest7(string result, string? baseUrl, string? relPathFormat, object[] items)
        // {
        //     const string relLabel = "rel";

        //     var caught = Assert.Throws<ArgumentException>(() => baseUrl.AddFormattedLink(relLabel, relPathFormat, items));

        //     Assert.Equal(result, caught.Message);
        // }

        // [Theory(DisplayName = "ToHrefTest (pass).")]
        // [InlineData("href")]
        // [InlineData("href2")]
        // [InlineData("href1")]
        // public void ToHrefTest(string rel)
        // {
        //     var links = new List<Link>
        //     {
        //         new Link{ Href="href",Rel="href"},
        //         new Link{ Href="href1",Rel="href1"},
        //         new Link{ Href="href2",Rel="href2"}
        //     };

        //     Assert.Equal(rel, links.ToHref(rel));
        // }
    }
}