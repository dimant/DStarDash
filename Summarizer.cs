﻿namespace DStarDash
{
    using DStarDash.Models;
    using DStarDash.Parsers;

    public class Summarizer
    {
        private IReflectorHtmlParser reflectorHtmlParser;

        public Summarizer(IReflectorHtmlParser reflectorHtmlParser)
        {
            this.reflectorHtmlParser = reflectorHtmlParser ?? throw new ArgumentNullException(nameof(reflectorHtmlParser));
        }

        public List<StatsRow> Summarize(IDictionary<string, List<ReflectorModule>> reflectors)
        {
            var result = new List<StatsRow>();

            foreach (var key in reflectors.Keys)
            {
                var name = reflectors[key].First().Name;
                var path = $"{name}.html";
                var location = reflectors[key].Select(x => x.Location).First();

                if (!File.Exists(path))
                {
                    var status = Status.Fail;
                    var statsRow = new StatsRow(name, location, status, 0, DateTime.MinValue, "");
                    result.Add(statsRow);
                }
                else
                {
                    var reflector = this.reflectorHtmlParser.ParseFromFile(path);

                    var heardOnCounts = new Dictionary<string, int>();

                    foreach (var heardUser in reflector?.HeardUsers ?? new List<ReflectorHeardUser>())
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

                    var nHeard = reflector?.HeardUsers.Count();
                    ReflectorHeardUser? last = null;
                    
                    if (nHeard > 0)
                    {
                        last = reflector?.HeardUsers.MaxBy(x => x.LastHeard);
                    }

                    if (reflector != null)
                    {
                        var statsRow = new StatsRow(
                            reflector.Name,
                            location,
                            Status.OK,
                            nHeard ?? 0,
                            last?.LastHeard ?? DateTime.MinValue,
                            busiestModule);
                        result.Add(statsRow);
                    }
                }
            }

            return result;
        }
    }
}
