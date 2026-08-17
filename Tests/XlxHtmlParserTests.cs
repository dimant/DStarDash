namespace DStarDash.Tests
{
    using DStarDash.Parsers;
    using Xunit;

    public class XlxHtmlParserTests
    {
        private static string FixturePath(string name)
            => Path.Combine(AppContext.BaseDirectory, "sample-data", name);

        [Fact]
        public void ParsesReflectorNameFromTitle()
        {
            var parser = new XlxHtmlParser();

            var reflector = parser.ParseFromFile(FixturePath("xlx-reflector.html"));

            Assert.NotNull(reflector);
            Assert.Equal("XLX801", reflector!.Name);
        }

        [Fact]
        public void ParsesAllHeardUsers()
        {
            var parser = new XlxHtmlParser();

            var reflector = parser.ParseFromFile(FixturePath("xlx-reflector.html"));

            Assert.NotNull(reflector);
            Assert.Equal(31, reflector!.HeardUsers.Count());
        }

        [Fact]
        public void PopulatesHeardUserFields()
        {
            var parser = new XlxHtmlParser();

            var reflector = parser.ParseFromFile(FixturePath("xlx-reflector.html"));

            var first = reflector!.HeardUsers.First();
            Assert.Equal("OE3MJA", first.Callsign);
            Assert.Equal("A", first.HeardOn);
            Assert.Equal(new DateTime(2022, 8, 28, 17, 35, 0), first.LastHeard);
        }
    }
}
