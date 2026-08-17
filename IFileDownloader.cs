namespace DStarDash
{
    public interface IFileDownloader
    {
        Task DownloadFileAsync(string uri, string path);
    }
}
