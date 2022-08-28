namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public class RefHtmlParser : IReflectorHtmlParser
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

        public Reflector? Parse(HtmlDocument doc)
        {
            var gwinfo = doc.DocumentNode
                .SelectSingleNode("//table/tr/td/table[@id='navigation']/tr/td[contains(.,'DREFD version')]/strong");

            if (gwinfo == null)
            {
                return null;
            }

            var version = gwinfo.InnerText;

            var refsysname = doc.DocumentNode.SelectSingleNode("//table/tr/td/table/tr/td/strong[contains(.,'Reflector System')]").InnerHtml;
            var name = refsysname.Split(' ')[0];

            var tables = doc.DocumentNode.SelectNodes("*//table/tr/td/table").ToArray();
            var heardTable = tables[4];

            var heardUsers = ParseHeardUsers(heardTable);

            return new Reflector(name, version, heardUsers);
        }

        private IEnumerable<ReflectorHeardUser> ParseHeardUsers(HtmlNode remoteTable)
        {
            var remoteUsers = new List<ReflectorHeardUser>();

            foreach (var row in remoteTable.SelectNodes(".//tr"))
            {
                var x = row.SelectNodes("td/span");

                if (x != null)
                {
                    var remoteUser = new ReflectorHeardUser();
                    var cols = x.Select(y => y.InnerText == "&nbsp;" ? string.Empty : y.InnerText).ToArray();

                    if (cols[0] != string.Empty)
                    {
                        remoteUser.Callsign = cols[0];
                    }

                    if (cols[1] != string.Empty)
                    {
                        remoteUser.Message = cols[1];
                    }

                    if (cols[2] != string.Empty)
                    {
                        remoteUser.HeardOn = cols[2];
                    }

                    if (cols[3] != string.Empty)
                    {
                        remoteUser.HeardAt = DateTime.Parse(cols[3]);
                    }

                    remoteUsers.Add(remoteUser);
                }
            }

            return remoteUsers;
        }
    }
}
