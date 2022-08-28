namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public interface IReflectorListHtmlParser
    {
        IEnumerable<ReflectorModule> Parse(HtmlDocument doc);
        IEnumerable<ReflectorModule> ParseFromFile(string path);
        IEnumerable<ReflectorModule> ParseFromUrl(string url);
    }
}