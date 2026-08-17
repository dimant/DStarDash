namespace DStarDash.Tests
{
    using DStarDash.Parsers;
    using Xunit;

    public class XlxParsingConfigTests
    {
        private readonly XlxParsingConfig config = new XlxParsingConfig();

        [Theory]
        [InlineData("28.08.2022 17:35", 2022, 8, 28, 17, 35)]
        [InlineData("2022-08-28 17:35", 2022, 8, 28, 17, 35)]
        [InlineData("08.28.2022 17:35", 2022, 8, 28, 17, 35)]
        [InlineData("2022.08.28 17:35:00", 2022, 8, 28, 17, 35)]
        [InlineData("August 8, 2026 14:18", 2026, 8, 8, 14, 18)]
        public void ParsesKnownDateFormats(string input, int y, int mo, int d, int h, int mi)
        {
            Assert.Equal(new DateTime(y, mo, d, h, mi, 0), config.ParseDate(input));
        }

        [Fact]
        public void ThrowsOnUnparseableDate()
        {
            Assert.ThrowsAny<FormatException>(() => config.ParseDate("not a date"));
        }

        [Fact]
        public void DefaultsIncludeEnglishColumnHeaders()
        {
            Assert.Contains("Callsign", config.CallsignHeaders);
            Assert.Contains("Last heard", config.LastHeardHeaders);
        }

        [Fact]
        public void LoadReturnsDefaultsWhenFileMissing()
        {
            var loaded = XlxParsingConfig.Load(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

            Assert.Contains("Callsign", loaded.CallsignHeaders);
        }

        [Fact]
        public void LoadReadsOverridesFromJson()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
            File.WriteAllText(path, "{\"DateFormats\":[\"yyyy/MM/dd HH:mm\"]}");

            try
            {
                var loaded = XlxParsingConfig.Load(path);
                Assert.Equal(new DateTime(2022, 8, 28, 17, 35, 0), loaded.ParseDate("2022/08/28 17:35"));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
