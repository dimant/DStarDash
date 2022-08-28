namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    internal class XlxHtmlParser : IReflectorHtmlParser
    {
        Reflector? IReflectorHtmlParser.Parse(HtmlDocument doc)
        {
            throw new NotImplementedException();
        }

        Reflector? IReflectorHtmlParser.ParseFromFile(string path)
        {
            throw new NotImplementedException();
        }

        Reflector? IReflectorHtmlParser.ParseFromUrl(string url)
        {
            throw new NotImplementedException();
        }
    }
}
