namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    internal class XlxListHtmlParser : ReflectorListHtmlParser
    {
        public override IEnumerable<ReflectorModule> Parse(HtmlDocument doc)
        {
            var reflectorModules = new List<ReflectorModule>();

            var rows = doc.DocumentNode
                .SelectNodes("//table[@class='listingtable']/tr");

            foreach (var row in rows)
            {
                var nodes = row.SelectNodes("td");

                if (nodes == null)
                {
                    continue;
                }

                var columns = nodes.ToArray();

                var module = string.Empty;
                string url = string.Empty; ;

                var urlNode = columns[1].SelectSingleNode(".//a[@class='listinglink']");
                var location = columns[2].InnerText.Trim();
                var isDown = columns[3].InnerText.Contains("down.png");
                if (!isDown)
                {
                    url = urlNode.GetAttributeValue("href", "");
                }

                var name = urlNode.InnerText;

                var usage = columns[4].InnerText;

                var reflectorModule = new ReflectorModule()
                {
                    Name = name,
                    Module = module ?? string.Empty,
                    Location = location ?? string.Empty,
                    Usage = usage ?? string.Empty,
                    Url = url ?? string.Empty,
                };

                reflectorModules.Add(reflectorModule);
            }

            return reflectorModules;
        }
    }
}
