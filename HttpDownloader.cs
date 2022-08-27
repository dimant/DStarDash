namespace DStarDash
{
    public class HttpDownloader
    {
        public async Task DownloadFileAsync(string uri, string path)
        {
            var client = new HttpClient();
            var response = client.GetAsync(uri).Result;

            using (var fs = new FileStream(path, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }
}
