namespace DStarDash
{
    using DStarDash.Models;
    using DStarDash.Parsers;

    public class Summarizer
    {
        private IReflectorHtmlParser reflectorHtmlParser;

        private readonly string dataDirectory;

        public Summarizer(IReflectorHtmlParser reflectorHtmlParser, string dataDirectory = "")
        {
            this.reflectorHtmlParser = reflectorHtmlParser ?? throw new ArgumentNullException(nameof(reflectorHtmlParser));
            this.dataDirectory = dataDirectory ?? string.Empty;
        }

        public List<StatsRow> Summarize(IDictionary<string, List<ReflectorModule>> reflectors)
        {
            var result = new List<StatsRow>();

            foreach (var key in reflectors.Keys)
            {
                var name = reflectors[key].First().Name;
                var path = ReflectorFile.PathFor(this.dataDirectory, name);
                var location = reflectors[key].Select(x => x.Location).First();

                if (!File.Exists(path))
                {
                    result.Add(new StatsRow(name, location, Status.Fail, 0, DateTime.MinValue, ""));
                    continue;
                }

                // A single malformed reflector page (unknown date format, unexpected
                // markup) must not take down the whole run — treat it like a failed
                // download and mark just that reflector Fail.
                try
                {
                    var reflector = this.reflectorHtmlParser.ParseFromFile(path);

                    if (reflector == null)
                    {
                        result.Add(new StatsRow(name, location, Status.Fail, 0, DateTime.MinValue, ""));
                        continue;
                    }

                    var heardOnCounts = new Dictionary<string, int>();

                    foreach (var heardUser in reflector.HeardUsers)
                    {
                        if (!heardOnCounts.ContainsKey(heardUser.HeardOn))
                        {
                            heardOnCounts[heardUser.HeardOn] = 1;
                        }
                        else
                        {
                            heardOnCounts[heardUser.HeardOn] += 1;
                        }
                    }

                    string busiestModule = "";

                    if (heardOnCounts.Count() > 0)
                    {
                        busiestModule = heardOnCounts.MaxBy(x => x.Value).Key;
                    }

                    var nHeard = reflector.HeardUsers.Count();
                    ReflectorHeardUser? last = nHeard > 0
                        ? reflector.HeardUsers.MaxBy(x => x.LastHeard)
                        : null;

                    result.Add(new StatsRow(
                        reflector.Name,
                        location,
                        Status.OK,
                        nHeard,
                        last?.LastHeard ?? DateTime.MinValue,
                        busiestModule));
                }
                catch (Exception)
                {
                    result.Add(new StatsRow(name, location, Status.Fail, 0, DateTime.MinValue, ""));
                }
            }

            return result;
        }
    }
}
