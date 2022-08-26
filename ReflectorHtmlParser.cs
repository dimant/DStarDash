namespace DStarDash
{
    using DStarDash.Models;
    using HtmlAgilityPack;
    
    public class ReflectorHtmlParser
    {
        public IEnumerable<ReflectorModule> ParseModulesFromFile(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(@"reflectors.html");

            return this.ParseModules(doc);
        }

        public IEnumerable<ReflectorModule> ParseModulesFromUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return this.ParseModules(doc);
        }

        public IEnumerable<ReflectorModule> ParseModules(HtmlDocument doc)
        {
            var reflectorModules = new List<ReflectorModule>();

            var rows = doc.DocumentNode.SelectNodes("//table[@id='ListView1_itemPlaceholderContainer']/tr");
            foreach (var row in rows)
            {
                var module = row.SelectSingleNode(".//span[contains(@id,'ReflectorLabel')]")?.InnerText;
                var usage = row.SelectSingleNode(".//span[contains(@id,'UsageLabel')]")?.InnerText;

                var status = row
                    .SelectSingleNode(".//span[contains(@id,'LinksLabel')]/a[.='Status']")?
                    .GetAttributeValue("href", "");

                var speed = row.SelectSingleNode(".//span[contains(@id,'SpeedLabel')]")?.InnerText;

                var reflectorModule = new ReflectorModule()
                {
                    Module = module ?? String.Empty,
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
