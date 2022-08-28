namespace DStarDash
{
    using DStarDash.Parsers;
    using ConsoleTables;
    using DStarDash.Models;

    internal class Program
    {
        private static ReflectorAggregator? aggregator;

        private static Summarizer? summarizer;

        public enum ReflectorType
        {
            Ref,
            Xlx
        }

        static void Main(string[] args)
        {
            var parser = new XlxListHtmlParser();

            parser.ParseFromFile("sample-data/xlx-listing.html");
        }

        static void xMain(ReflectorType reflectorType, bool download, string sortby, int top)
        {
            switch (reflectorType)
            {
                case ReflectorType.Ref:
                    aggregator = new ReflectorAggregator("ref-reflectors.html", new RefListHtmlParser());
                    summarizer = new Summarizer(new RefHtmlParser());
                    break;
                case ReflectorType.Xlx:
                    aggregator = new ReflectorAggregator("xlx-reflectors.html", new XlxListHtmlParser());
                    summarizer = new Summarizer(new XlxHtmlParser());
                    break;
            }

            if(download)
            {
                using (var progressBar = new ProgressBar("Downloading: "))
                {
                    Action<int, int> progress = (i, n) => progressBar.Report((double)i / n);
                    aggregator?.DownloadReflectorData(progress);
                }
            }

            PrintStats(sortby, top);
        }

        public static void PrintStats(string sortby, int top)
        {
            if (!File.Exists(aggregator?.ReflectorsPath))
            {
                throw new Exception($"{aggregator?.ReflectorsPath} does not exist, download data first.");
            }

            var reflectors = aggregator?.ReflectorsFromFile(aggregator.ReflectorsPath);
            if (reflectors == null)
            {
                return;
            }

            var summary = summarizer?.Summarize(reflectors) ?? new List<StatsRow>();

            if (!string.IsNullOrEmpty(sortby))
            {
                switch (sortby)
                {
                    case "Status":
                        summary.Sort((x, y) => x.Status.CompareTo(y.Status));
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

            var table = new ConsoleTable("","Name", "Location", "Status", "Heard", "Last", "Busiest");

            int i = 1;
            foreach (var s in summary)
            {
                table.AddRow(i++, s.Name, s.Location, s.Status, s.HeardUsers, s.LastHeard.ToLocalTime(), s.BusiestModule);
            }
        
            table.Write();
            Console.WriteLine();
        }
    }
}