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
                
                if (!File.Exists(path))
                {
                    var name = ReflectorAggregator.ReflectorNameFromUrl(key);
                    var status = Status.Fail;
                    var statsRow = new StatsRow(name, status, 0, 0, 0, DateTime.MinValue);
                    result.Add(statsRow);
                }
                else
                {
                    var reflector = reflectorParser.ParseFromFile(path);

                    var nGateways =
                        reflector?.LinkedGateways["Module A"].Count()
                        + reflector?.LinkedGateways["Module B"].Count()
                        + reflector?.LinkedGateways["Module C"].Count()
                        + reflector?.LinkedGateways["Module D"].Count();

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
                            Status.OK,
                            nGateways ?? 0,
                            nRemote ?? 0,
                            nHeard ?? 0,
                            last ?? DateTime.MinValue);
                        result.Add(statsRow);
                    }
                }
            }

            return result;
        }
    }
}
