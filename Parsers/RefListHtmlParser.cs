namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public class RefListHtmlParser : ReflectorListHtmlParser
    {
        public override IEnumerable<ReflectorModule> Parse(HtmlDocument doc, string uri)
        {
            var reflectorModules = new List<ReflectorModule>();

            var rows = doc.DocumentNode
                .SelectNodes("//table[@id='ListView1_itemPlaceholderContainer']/tr");

            foreach (var row in rows)
            {
                if (row.SelectNodes(".//td") == null)
                {
                    continue;
                }

                var module = row.SelectSingleNode(".//span[contains(@id,'ReflectorLabel')]")?.InnerText;
                var usage = row.SelectSingleNode(".//span[contains(@id,'UsageLabel')]")?.InnerText;
                var location = row.SelectSingleNode(".//span[contains(@id,'LocationLabel')]")?.InnerText;

                var url = row
                    .SelectSingleNode(".//span[contains(@id,'LinksLabel')]/a[.='Status']")?
                    .GetAttributeValue("href", "");

                var parsedUri = new Uri(url ?? string.Empty);
                var name = parsedUri.Host.Split('.').First();

                var reflectorModule = new ReflectorModule()
                {
                    Module = module ?? string.Empty,
                    Name = name.ToUpper(),
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
