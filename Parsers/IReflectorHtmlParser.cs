namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public interface IReflectorHtmlParser
    {
        Reflector? ParseFromFile(string path);

        Reflector? ParseFromUrl(string url);

        Reflector? Parse(HtmlDocument doc, string uri);
    }
}