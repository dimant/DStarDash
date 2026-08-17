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

        // Regression: this real XLX722 dashboard reports timestamps as
        // "August 8, 2026 14:18". That long-month format was not recognized and
        // the FormatException propagated out of Summarize, crashing a live run
        // over all reflectors. See xlx-longdate-reflector.html.
        [Fact]
        public void ParsesLongMonthDateFormatFromRealDashboard()
        {
            var parser = new XlxHtmlParser();

            var reflector = parser.ParseFromFile(FixturePath("xlx-longdate-reflector.html"));

            Assert.NotNull(reflector);
            Assert.Equal("XLX722", reflector!.Name);
            Assert.Equal(11, reflector.HeardUsers.Count());

            var first = reflector.HeardUsers.First();
            Assert.Equal("PY2PAO", first.Callsign);
            Assert.Equal("C", first.HeardOn);
            Assert.Equal(new DateTime(2026, 8, 8, 14, 18, 0), first.LastHeard);
        }

        // Regression: many reflector URLs serve a non-dashboard page (here, a real
        // meta-refresh redirect). These match the non-dashboard allowlist, so the
        // parser must return null rather than throwing "Couldn't find heard users
        // table". Summarizer then records them as Fail (see SummarizerTests).
        [Fact]
        public void ReturnsNullForNonDashboardRedirectPage()
        {
            var parser = new XlxHtmlParser();

            var reflector = parser.ParseFromFile(FixturePath("xlx-redirect-reflector.html"));

            Assert.Null(reflector);
        }
    }
}
