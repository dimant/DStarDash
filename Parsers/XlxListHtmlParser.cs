namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    internal class XlxListHtmlParser : IReflectorListHtmlParser
    {
        public IEnumerable<ReflectorModule> Parse(HtmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReflectorModule> ParseFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReflectorModule> ParseFromUrl(string url)
        {
            throw new NotImplementedException();
        }
    }
}
