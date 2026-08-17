namespace DStarDash
{
    public class HttpDownloader
    {
        private static readonly HttpClient SharedClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private readonly HttpClient client;

        public HttpDownloader() : this(SharedClient)
        {
        }

        internal HttpDownloader(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task DownloadFileAsync(string uri, string path)
        {
            using var response = await this.client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            using var fs = new FileStream(path, FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }
    }
}
