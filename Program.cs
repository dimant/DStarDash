namespace DStarDash
{
    using DStarDash.Parsers;
    using ConsoleTables;
    using DStarDash.Models;

    internal class Program
    {
        public enum ReflectorType
        {
            Ref,
            Xlx
        }

        static void Main(ReflectorType type, bool download, string sortby, int top)
        {
            var (aggregator, summarizer) = Build(type);

            if (download)
            {
                using (var progressBar = new ProgressBar("Downloading: "))
                {
                    Action<int, int> progress = (i, n) => progressBar.Report((double)i / n);
                    aggregator.DownloadReflectorData(progress);
                }
            }

            PrintStats(aggregator, summarizer, sortby, top);
        }

        public const string DataDirectory = "data";

        public static (ReflectorAggregator, Summarizer) Build(ReflectorType type)
        {
            switch (type)
            {
                case ReflectorType.Xlx:
                    return (
                        new ReflectorAggregator(
                            Path.Combine(DataDirectory, "xlx-reflectors.html"),
                            "http://oe1phs.ddns.net/db/index.php?show=reflectors",
                            new XlxListHtmlParser()),
                        new Summarizer(new XlxHtmlParser(), DataDirectory));
                case ReflectorType.Ref:
                default:
                    return (
                        new ReflectorAggregator(
                            Path.Combine(DataDirectory, "ref-reflectors.html"),
                            "http://apps.dstarinfo.com/reflectors.aspx",
                            new RefListHtmlParser()),
                        new Summarizer(new RefHtmlParser(), DataDirectory));
            }
        }

        public static void PrintStats(
            ReflectorAggregator aggregator, Summarizer summarizer, string sortby, int top)
        {
            if (!File.Exists(aggregator.ReflectorsPath))
            {
                throw new Exception($"{aggregator.ReflectorsPath} does not exist, download data first.");
            }

            var reflectors = aggregator.ReflectorsFromFile(aggregator.ReflectorsPath);

            var summary = Sort(summarizer.Summarize(reflectors), sortby);

            if (top > 0)
            {
                summary = summary.Take(top).ToList();
            }

            var table = new ConsoleTable("", "Name", "Location", "Status", "Heard", "Last", "Busiest");

            int i = 1;
            foreach (var s in summary)
            {
                table.AddRow(i++, s.Name, s.Location, s.Status, s.HeardUsers, s.LastHeard.ToLocalTime(), s.BusiestModule);
            }

            table.Write();
            Console.WriteLine();
        }

        internal static List<StatsRow> Sort(List<StatsRow> summary, string sortby)
        {
            if (string.IsNullOrEmpty(sortby))
            {
                return summary;
            }

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

            return summary;
        }
    }
}
