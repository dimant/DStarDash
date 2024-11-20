namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public class ReflectorListHtmlParser : IReflectorListHtmlParser
    {
        public virtual IEnumerable<ReflectorModule> ParseFromFile(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            return Parse(doc, path);
        }

        public IEnumerable<ReflectorModule> ParseFromUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return Parse(doc, url);
        }

        public virtual IEnumerable<ReflectorModule> Parse(HtmlDocument doc, string uri)
        {
            throw new NotImplementedException();
        }
    }
}
