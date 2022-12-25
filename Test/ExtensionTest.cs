using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class ExtensionTest
    {
        const string rel = "rel";
        const string baseUrl = "https://foo.bar";

        [Theory(DisplayName = "HttpContext.AddLink (pass)")]
        [InlineData("https://foo.bar/dingle/ball?value1=1&value2=2", "dingle/ball?value1=1&value2=2")]
        [InlineData("https://foo.bar/dingle", "dingle")]
        public void AddLinkHttpContextPassTest(string result, string relativeUrl)
        {
            var links = GetHttpContext()
                .AddLink(rel, relativeUrl)
                .Build();

            Assert.Equal(result, links.First().Href);
            Assert.Equal(rel, links.First().Rel);
        }

        [Theory(DisplayName = "String.AddLink (pass)")]
        [InlineData("https://foo.bar/dingle/ball?value1=1&value2=2", "dingle/ball?value1=1&value2=2")]
        [InlineData("https://foo.bar/dingle", "dingle")]
        public void AddLinkStringPassTest(string result, string relativeUrl)
        {
            var links = baseUrl
                .AddLink(rel, relativeUrl)
                .Build();

            Assert.Equal(result, links.First().Href);
            Assert.Equal(rel, links.First().Rel);
        }

        [Theory(DisplayName = "HttpContext.AddLink (fail)")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", null, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "", "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", "\t", "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", rel, null)]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", rel, "")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", rel, "\t")]
        public void AddLinkHttpContextFailTest(string result, string rel, string relativeUrl)
        {
            var ex = Assert.Throws<ArgumentException>(() => GetHttpContext().AddLink(rel, relativeUrl));

            Assert.Equal(result, ex.Message);
        }

        [Theory(DisplayName = "String.AddLink (fail)")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", null, rel, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "", rel, "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "\t", rel, "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", baseUrl, null, "dingle/ball?value1=1&value2=2")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", baseUrl, "", "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')", baseUrl, "\t", "dingle")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", baseUrl, rel, null)]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", baseUrl, rel, "")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relativeUrl')", baseUrl, rel, "\t")]
        public void AddLinkStringFailTest(string result,string baseUrl, string label, string relativeUrl)
        {
            var ex = Assert.ThrowsAny<ArgumentException>(() => baseUrl.AddLink(label, relativeUrl));

            Assert.Equal(result, ex.Message);
        }

        static TestHttpContext GetHttpContext()
        {
            return new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });
        }

        [Theory(DisplayName = "HttpClient.AddLink (passx).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest3(string result, string relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var http = new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });

            var linkbuilder = http.AddFormattedLink(relLabel, relPathFormat, items);

            Assert.Equal(new Link
            {
                Href = result,
                Rel = relLabel,
            }, linkbuilder.Build().First());
        }

        [Theory(DisplayName = "HttpContext.AddLink (failx).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest4(string result, string? relPathFormat, object[] items)
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


        [Theory(DisplayName = "BaseUrl.AddLink (pass).")]
        [InlineData("https://foo.bar/Dingle", "Dingle", new object[] { })]
        [InlineData("https://foo.bar/DingleBall", "Dingle{0}", new object[] { "Ball" })]
        [InlineData("https://foo.bar/DingleBalldingle", "Dingle{0}{1}", new object[] { "Ball", "dingle" })]
        [InlineData("https://foo.bar/DingleBalldingle2", "Dingle{0}{1}{2}", new object[] { "Ball", "dingle", 2 })]
        public void AddLinkTest5(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";
            const string baseUrl = "https://foo.bar";

            var output = baseUrl.AddFormattedLink(relLabel, relPathFormat, items).Build().First();
            var expected = new Link
            {
                Href = result,
                Rel = "rel"
            };

            Assert.Equal(expected.Href, output.Href);
            Assert.Equal(expected.Rel, output.Rel);
        }

        [Theory(DisplayName = "BaseUrl.AddLink (fail 1).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "", "relPathFormat", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", null, "relPathFormat", new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "\t", "relPathFormat", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest6(string result, string? baseUrl, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var caught = Assert.Throws<ArgumentException>(() => baseUrl.AddFormattedLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "BaseUrl.AddLink (fail 2).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest7(string result, string? baseUrl, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var caught = Assert.Throws<ArgumentException>(() => baseUrl.AddFormattedLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "ToHrefTest (pass).")]
        [InlineData("href")]
        [InlineData("href2")]
        [InlineData("href1")]
        public void ToHrefTest(string rel)
        {
            var links = new List<Link>
            {
                new Link{ Href="href",Rel="href"},
                new Link{ Href="href1",Rel="href1"},
                new Link{ Href="href2",Rel="href2"}
            };

            Assert.Equal(rel, links.ToHref(rel));
        }
    }
}