using MeyerCorp.HateoasBuilder;
using System;
using Xunit;

namespace MeyerCorp.HateoasBuilder.Test
{
    public class LinkBuilderTest
    {
        [Fact(DisplayName = "LinkBuilderTest: Check RelHrefPairs property.")]
        public void LinkBuilder1Test()
        {
            var test = new LinkBuilder(String.Empty);

            Assert.Empty(test.GetLinks());
        }

        [Fact(DisplayName = "LinkBuilderTest: Check null/empty parameters.")]
        public void LinkBuilder2Test()
        {
            var test = new LinkBuilder(String.Empty);

            Assert.Throws<ArgumentNullException>(() => test.AddLink(null, "asdf", "test"));
            Assert.Throws<ArgumentNullException>(() => test.AddLink("asdf", null, "test"));
            Assert.Throws<ArgumentNullException>(() => test.AddLink(string.Empty, "asdf", "test"));
            Assert.Throws<ArgumentNullException>(() => test.AddLink("asdf", string.Empty, "test"));
            Assert.Throws<ArgumentNullException>(() => test.AddLink("asdf", null, "null"));
            Assert.Throws<ArgumentNullException>(() => test.AddLink("asdf", string.Empty, "null"));
        }

        [Fact(DisplayName = "LinkBuilderTest: Check single link.")]
        public void LinkBuilder4Test()
        {
            var test = new LinkBuilder("http://foo.bar");

            test.AddLink("rel", "asdf{0}", "uno");
            Assert.Single(test.GetLinks());
            Assert.Equal(new Link
            {
                Href = "http://foo.bar/asdfuno",
                Rel = "rel",
            }, test[0]);

            test = new LinkBuilder("http://foo.bar");
            test.AddLink("rel", "asdf{0}{1}", "uno", "dos");
            Assert.Single(test.GetLinks());
            Assert.Equal(new Link
            {
                Href = "http://foo.bar/asdfunodos",
                Rel = "rel",
            }, test[0]);

            test = new LinkBuilder("http://foo.bar");
            test.AddLink("rel", "asdf{0}{1}{2}", "test", "dos", "tres");
            Assert.Single(test.GetLinks());
            Assert.Equal(new Link
            {
                Href = "http://foo.bar/asdftestdostres",
                Rel = "rel",
            }, test[0]);
        }
    }
}