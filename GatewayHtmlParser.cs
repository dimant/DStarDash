namespace DStarDash
{
    using DStarDash.Models;
    using HtmlAgilityPack;

    public class GatewayHtmlParser
    {
        public Gateway ParseGateway(HtmlDocument doc)
        {
            var gwinfo = doc.DocumentNode
                .SelectSingleNode("//table/tr/td/table[@id='navigation']/tr/td[contains(.,'DREFD version')]/strong");

            if (gwinfo == null)
            {
                return null;
            }

            var version = gwinfo.InnerText;

            var tables = doc.DocumentNode.SelectNodes("*//table/tr/td/table").ToArray();
            var remoteTable = tables[3];
            var heardTable = tables[4];

            var remoteUsers = ParseRemoteUsers(remoteTable);
            var heardUsers = ParseHeardUsers(heardTable);

            return new Gateway(version, remoteUsers, heardUsers);
        }

        private IEnumerable<GatewayRemoteUser> ParseRemoteUsers(HtmlNode remoteTable)
        {
            var remoteUsers = new List<GatewayRemoteUser>();

            foreach (var row in remoteTable.SelectNodes(".//tr"))
            {
                var x = row.SelectNodes("td/span");

                if (x != null)
                {
                    var remoteUser = new GatewayRemoteUser();
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

        private IEnumerable<GatewayHeardUser> ParseHeardUsers(HtmlNode remoteTable)
        {
            var remoteUsers = new List<GatewayHeardUser>();

            foreach (var row in remoteTable.SelectNodes(".//tr"))
            {
                var x = row.SelectNodes("td/span");

                if (x != null)
                {
                    var remoteUser = new GatewayHeardUser();
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
