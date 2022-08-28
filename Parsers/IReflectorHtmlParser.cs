namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public interface IReflectorHtmlParser
    {
        Reflector? Parse(HtmlDocument doc);

        Reflector? ParseFromFile(string path);
        
        Reflector? ParseFromUrl(string url);
    }
}