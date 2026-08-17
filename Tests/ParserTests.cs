namespace DStarDash.Tests
{
    using DStarDash.Parsers;
    using Xunit;

    public class RefHtmlParserTests
    {
        private static string Fixture(string name)
            => Path.Combine(AppContext.BaseDirectory, "sample-data", name);

        [Fact]
        public void ParsesReflectorName()
        {
            var reflector = new RefHtmlParser().ParseFromFile(Fixture("ref-reflector.html"));

            Assert.NotNull(reflector);
            Assert.Equal("REF001", reflector!.Name);
        }

        [Fact]
        public void ParsesHeardUsers()
        {
            var reflector = new RefHtmlParser().ParseFromFile(Fixture("ref-reflector.html"));

            Assert.NotEmpty(reflector!.HeardUsers);
        }
    }

    public class ListingParserTests
    {
        private static string Fixture(string name)
            => Path.Combine(AppContext.BaseDirectory, "sample-data", name);

        [Fact]
        public void RefListingSkipsRowsWithoutStatusUrlInsteadOfThrowing()
        {
            var modules = new RefListHtmlParser().ParseFromFile(Fixture("ref-listing.html")).ToList();

            Assert.NotEmpty(modules);
            Assert.Equal("REF001A", modules.First().Module);
            Assert.All(modules, m => Assert.False(string.IsNullOrEmpty(m.Url)));
        }

        [Fact]
        public void XlxListingParsesReflectors()
        {
            var modules = new XlxListHtmlParser().ParseFromFile(Fixture("xlx-listing.html")).ToList();

            Assert.NotEmpty(modules);
            Assert.All(modules, m => Assert.False(string.IsNullOrEmpty(m.Name)));
        }
    }
}
