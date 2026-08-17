namespace DStarDash.Tests
{
    using DStarDash;
    using DStarDash.Models;
    using DStarDash.Parsers;
    using HtmlAgilityPack;
    using Xunit;

    public class SummarizerTests
    {
        private sealed class ThrowingParser : IReflectorHtmlParser
        {
            public Reflector? Parse(HtmlDocument doc, string uri) => throw new FormatException("bad page");
            public Reflector? ParseFromFile(string path) => throw new FormatException("bad page");
            public Reflector? ParseFromUrl(string url) => throw new FormatException("bad page");
        }

        [Fact]
        public void ParseFailureMarksReflectorFailInsteadOfCrashingWholeRun()
        {
            var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(dir);

            try
            {
                File.WriteAllText(ReflectorFile.PathFor(dir, "XLX999"), "<html>unparseable</html>");

                var reflectors = new Dictionary<string, List<ReflectorModule>>
                {
                    ["http://x.test"] = new()
                    {
                        new ReflectorModule { Name = "XLX999", Url = "http://x.test", Location = "Nowhere" },
                    },
                };

                var summarizer = new Summarizer(new ThrowingParser(), dir);

                var rows = summarizer.Summarize(reflectors);

                var row = Assert.Single(rows);
                Assert.Equal("XLX999", row.Name);
                Assert.Equal(Status.Fail, row.Status);
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
