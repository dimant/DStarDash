namespace DStarDash
{
    using HtmlAgilityPack;

    internal class Program
    {
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load(@"gateway.html");
            var parser = new GatewayHtmlParser();

            var gateway = parser.ParseGateway(doc);
        }
    }
}