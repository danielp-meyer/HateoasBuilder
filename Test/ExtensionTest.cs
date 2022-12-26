using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class ExtensionTest
    {
        protected const string rel = "rel";
        protected const string baseUrl = "https://foo.bar";


        protected static TestHttpContext GetHttpContext()
        {
            return new TestHttpContext(new TestHttpRequest
            {
                Scheme = "https",
                Host = new HostString("foo.bar"),
            });
        }

        [Theory(DisplayName = "ToHrefTest (pass).")]
        [InlineData("href")]
        [InlineData("href2")]
        [InlineData("href1")]
        public void ToHrefTestPass(string rel)
        {
            var links = new List<Link>
            {
                new Link(rel+"x",rel),
                new Link(rel,rel),
                new Link(rel+"xx",rel)
            };

            Assert.Equal(rel, links.ToHref(rel));
        }

        [Theory(DisplayName = "ToHrefTest (fail).")]
        [InlineData("href")]
        public void ToHrefTestFail(string rel)
        {
            var links = new List<Link>
            {
                new Link(rel,rel),
                new Link(rel,rel),
                new Link(rel,rel)
            };

            var ex = Assert.Throws<InvalidOperationException>(() => Assert.Equal(rel, links.ToHref(rel)));

            Assert.Equal("Sequence contains more than one matching element", ex.Message);
        }
    }
}