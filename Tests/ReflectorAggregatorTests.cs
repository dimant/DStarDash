namespace DStarDash.Tests
{
    using DStarDash.Models;
    using DStarDash.Parsers;
    using Xunit;

    public class ReflectorAggregatorTests
    {
        private sealed class FakeDownloader : IFileDownloader
        {
            private readonly Func<string, bool> succeeds;

            public FakeDownloader(Func<string, bool> succeeds) => this.succeeds = succeeds;

            public Task DownloadFileAsync(string uri, string path)
            {
                if (!succeeds(uri))
                {
                    throw new HttpRequestException("boom");
                }

                return Task.CompletedTask;
            }
        }

        private static ReflectorAggregator NewAggregator()
            => new ReflectorAggregator("list.html", "http://list.test", new XlxListHtmlParser());

        private static IDictionary<string, List<ReflectorModule>> TwoReflectors()
            => new Dictionary<string, List<ReflectorModule>>
            {
                ["http://good.test"] = new() { new ReflectorModule { Name = "GOOD", Url = "http://good.test" } },
                ["http://bad.test"] = new() { new ReflectorModule { Name = "BAD", Url = "http://bad.test" } },
            };

        [Fact]
        public void DownloadReflectorsReturnsNamesThatFailed()
        {
            var failures = NewAggregator().DownloadReflectors(
                TwoReflectors(),
                new FakeDownloader(uri => uri.Contains("good")),
                null);

            Assert.Contains("BAD", failures);
            Assert.DoesNotContain("GOOD", failures);
        }

        [Fact]
        public void DownloadReflectorsReportsProgressForEveryReflector()
        {
            int calls = 0;
            var lockObj = new object();

            NewAggregator().DownloadReflectors(
                TwoReflectors(),
                new FakeDownloader(_ => true),
                (done, total) =>
                {
                    lock (lockObj)
                    {
                        calls++;
                        Assert.Equal(2, total);
                    }
                });

            Assert.Equal(2, calls);
        }
    }
}
