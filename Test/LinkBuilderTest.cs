using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class LinkBuilderTest
    {
        [Theory(DisplayName = "Constructor (fail).")]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(null)]
        public void ConstructorFailTest(string baseUrl)
        {
            var caught = Assert.Throws<ArgumentNullException>(() => new LinkBuilder(baseUrl));

            Assert.Equal("Value cannot be null. (Parameter 'baseUrl')", caught.Message);
        }

        [Theory(DisplayName = "Check null/empty parameters.")]
        [InlineData(null, "asdf", new object[] { "test" }, "Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')")]
        [InlineData("asdf", null, new object[] { "test" }, "Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')")]
        [InlineData("", "asdf", new object[] { "test" }, "Parameter cannot be null, empty or whitespace. (Parameter 'relLabel')")]
        [InlineData("asdf", "", new object[] { "test" }, "Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')")]
        [InlineData("asdf", null, new object[] { }, "Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')")]
        [InlineData("asdf", "", new object[] { }, "Parameter cannot be null, empty or whitespace. (Parameter 'relPathFormat')")]
        public void LinkBuilder1Test(string? relLabel, string? relPathFormat, IEnumerable<object> formatItems, string message)
        {
            var test = new LinkBuilder("https:meyerus.com");

            var caught = Assert.Throws<ArgumentNullException>(() => test.AddLink(relLabel, relPathFormat, formatItems));

            Assert.Equal(message, caught.Message);
        }

        [Theory(DisplayName = "Check single link.")]
        [InlineData("http://foo.bar/asdfuno", "asdf{0}", new string[] { "uno" })]
        [InlineData("http://foo.bar/asdfunodos", "asdf{0}{1}", new string[] { "uno", "dos" })]
        [InlineData("http://foo.bar/asdftestdostres", "asdf{0}{1}{2}", new string[] { "test", "dos", "tres" })]
        public void LinkBuilder2Test(string result, string format, string[] items)
        {
            const string rel = "rel";
            var test = new LinkBuilder("http://foo.bar");

            test.AddLink(rel, format, items.ToArray());
            Assert.Single(test.GetLinks());
            Assert.Equal(new Link
            {
                Href = result,
                Rel = rel,
            }, test[0]);
        }
    }
}