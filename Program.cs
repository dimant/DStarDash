namespace DStarDash
{
    using ConsoleTables;

    internal class Program
    {
        static void Main(bool download, string sortby, int top)
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

            PrintStats(sortby, top);
        }

        public static void PrintStats(string sortby, int top)
        {
            var aggregator = new ReflectorAggregator();

            if (!File.Exists(aggregator.ReflectorsPath))
            {
                throw new Exception($"{aggregator.ReflectorsPath} does not exist, download data first.");
            }

            var reflectors = aggregator.ReflectorsFromFile(aggregator.ReflectorsPath);
            var summarizer = new Summarizer();
            var summary = summarizer.Summarize(reflectors);

            if (!string.IsNullOrEmpty(sortby))
            {
                switch (sortby)
                {
                    case "Status":
                        summary.Sort((x, y) => x.Status.CompareTo(y.Status));
                        break;
                    case "Gw":
                        summary.Sort((x, y) => y.LinkedGateways.CompareTo(x.LinkedGateways));
                        break;
                    case "Remote":
                        summary.Sort((x, y) => y.RemoteUsers.CompareTo(x.RemoteUsers));
                        break;
                    case "Heard":
                        summary.Sort((x, y) => y.HeardUsers.CompareTo(x.HeardUsers));
                        break;
                    case "Last":
                        summary.Sort((x, y) => y.LastHeard.CompareTo(x.LastHeard));
                        break;
                    default:
                        summary.Sort((x, y) => x.Name.CompareTo(y.Name));
                        break;
                }
            }

            if (top > 0)
            {
                summary = summary.Take(top).ToList();
            }

            var table = new ConsoleTable("Name", "Status", "Gw", "Remote", "Heard", "Last");

            foreach (var s in summary)
            {
                table.AddRow(s.Name, s.Status, s.LinkedGateways, s.RemoteUsers, s.HeardUsers, s.LastHeard);
            }

            table.Write();
            Console.WriteLine();
        }
    }
}