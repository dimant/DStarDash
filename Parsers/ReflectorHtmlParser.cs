namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public abstract class ReflectorHtmlParser : IReflectorHtmlParser
    {
        public Reflector? ParseFromFile(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            return Parse(doc);
        }

        public Reflector? ParseFromUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return Parse(doc);
        }

        public virtual Reflector? Parse(HtmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}
