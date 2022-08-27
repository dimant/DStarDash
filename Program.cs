namespace DStarDash
{
    internal class Program
    {
        static void Main(bool download)
        {
            if(download)
            {
                var aggregator = new ReflectorAggregator();

                using (var progressBar = new ProgressBar("Downloading: "))
                {
                    Action<int, int> progress = (i, n) => progressBar.Report((double)i / n);
                    aggregator.DownloadReflectorData(progress);
                }
            }
        }
    }
}