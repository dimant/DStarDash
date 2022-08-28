namespace DStarDash
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public class ReflectorHtmlParser
    {
        public Reflector? ParseFromFile(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            return this.Parse(doc);
        }

        public Reflector? ParseFromUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return this.Parse(doc);
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
            var remoteTable = tables[3];
            var heardTable = tables[4];

            var remoteUsers = ParseRemoteUsers(remoteTable);
            var heardUsers = ParseHeardUsers(heardTable);
            var linkedGateways = ParseLinkedGateways(doc);

            return new Reflector(name, version, linkedGateways, remoteUsers, heardUsers);
        }

        private IEnumerable<ReflectorRemoteUser> ParseRemoteUsers(HtmlNode remoteTable)
        {
            var remoteUsers = new List<ReflectorRemoteUser>();

            foreach (var row in remoteTable.SelectNodes(".//tr"))
            {
                var x = row.SelectNodes("td/span");

                if (x != null)
                {
                    var remoteUser = new ReflectorRemoteUser();
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
                        remoteUser.Type = cols[3];
                    }

                    remoteUsers.Add(remoteUser);
                }
            }

            return remoteUsers;
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

        private Dictionary<string, List<string>> ParseLinkedGateways(HtmlDocument doc)
        {
            var rows = doc.DocumentNode
                .SelectSingleNode("//table/tr/td/table/tr/td/table/tr/th/span[contains(.,'Module A')]")
                    .ParentNode  // th
                    .ParentNode  // tr
                    .ParentNode; // table

            var linkedgws = new Dictionary<string, List<string>>();
            linkedgws["Module A"] = new List<string>();
            linkedgws["Module B"] = new List<string>();
            linkedgws["Module C"] = new List<string>();
            linkedgws["Module D"] = new List<string>();
            linkedgws["Module E"] = new List<string>();

            foreach (var row in rows.SelectNodes(".//tr"))
            {
                var x = row.SelectNodes("td/span");

                if (x != null)
                {
                    var cols = x.Select(y => y.InnerText == "&nbsp;" ? string.Empty : y.InnerText).ToArray();

                    if (cols[0] != string.Empty)
                    {
                        linkedgws["Module A"].Add(cols[0]);
                    }
                    if (cols[1] != string.Empty)
                    {
                        linkedgws["Module B"].Add(cols[1]);
                    }
                    if (cols[2] != string.Empty)
                    {
                        linkedgws["Module C"].Add(cols[2]);
                    }
                    if (cols[3] != string.Empty)
                    {
                        linkedgws["Module D"].Add(cols[3]);
                    }
                    if (cols[4] != string.Empty)
                    {
                        linkedgws["Module E"].Add(cols[4]);
                    }
                }
            }

            return linkedgws;
        }
    }
}
