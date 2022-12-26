using System;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class AddFormattedLinkTests : ExtensionTest
    {
        [Theory(DisplayName = "HttpContext.AddFormattedLink (pass).")]
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
        [InlineData("Value cannot be null. (Parameter 'format')", null, new object[] { "ball", "dingle" })]
        public void AddFormattedLinkHttpContextFail(string result, string? relPathFormat, object[] items)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => GetHttpContext().AddFormattedLink(rel, relPathFormat, items));

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
        [InlineData("Value cannot be null. (Parameter 'format')", null, new object[] { "ball", "dingle" })]
        public void AddFormattedLinkStringFail(string result, string? relPathFormat, object[] items)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => GetHttpContext().AddFormattedLink(rel, relPathFormat, items));

            Assert.Equal(result, caught.Message);
        }
    }
}