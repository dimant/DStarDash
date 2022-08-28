namespace DStarDash
{
    using DStarDash.Models;

    public class Summarizer
    {
        public List<StatsRow> Summarize(IDictionary<string, List<ReflectorModule>> reflectors)
        {
            var reflectorParser = new ReflectorHtmlParser();

            var result = new List<StatsRow>();

            foreach (var key in reflectors.Keys)
            {
                var path = ReflectorAggregator.ReflectorPathFromUrl(key);
                var location = reflectors[key].Select(x => x.Location).First();

                if (!File.Exists(path))
                {
                    var name = ReflectorAggregator.ReflectorNameFromUrl(key);
                    var status = Status.Fail;
                    var statsRow = new StatsRow(name, location, status, 0, 0, 0, DateTime.MinValue, "");
                    result.Add(statsRow);
                }
                else
                {
                    var reflector = reflectorParser.ParseFromFile(path);

                    var aCount = reflector?.LinkedGateways["Module A"].Count() ?? 0;
                    var bCount = reflector?.LinkedGateways["Module B"].Count() ?? 0;
                    var cCount = reflector?.LinkedGateways["Module C"].Count() ?? 0;
                    var dCount = reflector?.LinkedGateways["Module D"].Count() ?? 0;


                    var nGateways = aCount + bCount + cCount + dCount;

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

                    var nRemote = reflector?.RemoteUsers.Count();
                    var nHeard = reflector?.HeardUsers.Count();
                    DateTime? last = DateTime.MinValue;
                    
                    if (nHeard > 0)
                    {
                        last = reflector?.HeardUsers.Max(x => x.HeardAt);
                    }

                    if (reflector != null)
                    {
                        var statsRow = new StatsRow(
                            reflector.Name,
                            location,
                            Status.OK,
                            nGateways,
                            nRemote ?? 0,
                            nHeard ?? 0,
                            last ?? DateTime.MinValue,
                            busiestModule);
                        result.Add(statsRow);
                    }
                }
            }

            return result;
        }
    }
}
