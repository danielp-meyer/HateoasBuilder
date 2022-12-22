using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class ExtensionTest
    {
        [Theory(DisplayName = "HttpClient.AddLink (pass).")]
        [InlineData("https://foo.bar/dingleball", "dingle{0}", new object[] { "ball" })]
        [InlineData("https://foo.bar/dingleballdingle", "dingle{0}{1}", new object[] { "ball", "dingle" })]
        [InlineData("https://foo.bar/dingleballdingle2", "dingle{0}{1}{2}", new object[] { "ball", "dingle", 2 })]
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

        [Theory(DisplayName = "HttpClient.AddLink (fail).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest1(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var http = new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });

            var caught = Assert.Throws<ArgumentNullException>(() => http.AddLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }


        [Theory(DisplayName = "BaseUrl.AddLink (pass).")]
        [InlineData("https://foo.bar/Dingle", "Dingle", new object[] { })]
        [InlineData("https://foo.bar/DingleBall", "Dingle{0}", new object[] { "Ball" })]
        [InlineData("https://foo.bar/DingleBalldingle", "Dingle{0}{1}", new object[] { "Ball", "dingle" })]
        [InlineData("https://foo.bar/DingleBalldingle2", "Dingle{0}{1}{2}", new object[] { "Ball", "dingle", 2 })]
        public void AddLinkTest2(string result, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";
            const string baseUrl = "https://foo.bar";

            var output = baseUrl.AddLink(relLabel, relPathFormat, items).GetLinks().First();
            var expected = new Link
            {
                Href = result,
                Rel = "rel"
            };

            Assert.Equal(expected.Href, output.Href);
            Assert.Equal(expected.Rel, output.Rel);
        }

        [Theory(DisplayName = "BaseUrl.AddLink (fail).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "", "relPathFormat", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", null, "relPathFormat", new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'baseUrl')", "\t", "relPathFormat", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest3(string result, string? baseUrl, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var caught = Assert.Throws<ArgumentException>(() => baseUrl.AddLink(relLabel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }

        [Theory(DisplayName = "BaseUrl.AddLink (fail2).")]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "", new object[] { "ball" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", null, new object[] { "ball", "dingle" })]
        [InlineData("Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')", "https://foo.bar", "\t", new object[] { "ball", "dingle", 2 })]
        public void AddLinkTest4(string result, string? baseUrl, string? relPathFormat, object[] items)
        {
            const string relLabel = "rel";

            var caught = Assert.Throws<ArgumentNullException>(() => baseUrl.AddLink(relLabel, relPathFormat, items));

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

            Assert.Equal(rel,links.ToHref(rel));
        }

        [Fact(DisplayName = "ToHrefTest (fail).")]
        public void ToHrefTest1()
        {
            var links = new List<Link>
            {
                new Link{ Href="href",Rel="href"},
                new Link{ Href="href1",Rel="href1"},
                new Link{ Href="href2",Rel="href2"}
            };

            Assert.Null(links.ToHref("no"));
        }
    }
}