namespace DStarDash
{
    using DStarDash.Models;
    using HtmlAgilityPack;
    
    public class ReflectorListHtmlParser
    {
        public IEnumerable<ReflectorModule> ParseFromFile(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            return this.Parse(doc);
        }

        public IEnumerable<ReflectorModule> ParseFromUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return this.Parse(doc);
        }

        public IEnumerable<ReflectorModule> Parse(HtmlDocument doc)
        {
            var reflectorModules = new List<ReflectorModule>();

            var rows = doc.DocumentNode
                .SelectNodes("//table[@id='ListView1_itemPlaceholderContainer']/tr/td")
                .Select(x => x.ParentNode);
            foreach (var row in rows)
            {
                var module = row.SelectSingleNode(".//span[contains(@id,'ReflectorLabel')]")?.InnerText;
                var usage = row.SelectSingleNode(".//span[contains(@id,'UsageLabel')]")?.InnerText;
                var location = row.SelectSingleNode(".//span[contains(@id,'LocationLabel')]")?.InnerText;

                var status = row
                    .SelectSingleNode(".//span[contains(@id,'LinksLabel')]/a[.='Status']")?
                    .GetAttributeValue("href", "");

                var speed = row.SelectSingleNode(".//span[contains(@id,'SpeedLabel')]")?.InnerText;

                var reflectorModule = new ReflectorModule()
                {
                    Module = module ?? String.Empty,
                    Location = location ?? string.Empty,
                    Usage = usage ?? String.Empty,
                    Status = status ?? String.Empty,
                    Speed = speed ?? String.Empty,
                };

                reflectorModules.Add(reflectorModule);
            }

            return reflectorModules;
        }
    }
}
