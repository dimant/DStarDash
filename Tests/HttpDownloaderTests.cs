namespace DStarDash.Tests
{
    using System.Net;
    using Xunit;

    public class HttpDownloaderTests
    {
        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage response;

            public StubHandler(HttpResponseMessage response) => this.response = response;

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(this.response);
        }

        private static HttpDownloader DownloaderReturning(HttpStatusCode status, string body)
        {
            var response = new HttpResponseMessage(status) { Content = new StringContent(body) };
            return new HttpDownloader(new HttpClient(new StubHandler(response)));
        }

        [Fact]
        public async Task WritesResponseBodyToFile()
        {
            var downloader = DownloaderReturning(HttpStatusCode.OK, "hello reflector");
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                await downloader.DownloadFileAsync("http://example.test/x", path);
                Assert.Equal("hello reflector", await File.ReadAllTextAsync(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public async Task ThrowsOnErrorStatusWithoutWritingFile()
        {
            var downloader = DownloaderReturning(HttpStatusCode.NotFound, "<html>not found</html>");
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            await Assert.ThrowsAnyAsync<HttpRequestException>(
                () => downloader.DownloadFileAsync("http://example.test/x", path));
            Assert.False(File.Exists(path));
        }
    }
}
