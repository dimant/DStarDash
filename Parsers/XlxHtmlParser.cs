namespace DStarDash.Parsers
{
    using DStarDash.Models;
    using HtmlAgilityPack;
    using System.Globalization;

    internal class XlxHtmlParser : ReflectorHtmlParser
    {
        private enum ColumnType
        {
            Callsign,
            HeardOn,
            LastHeard
        }

        private struct ColumnAssociation
        {
            public string Name { get; }

            public int Index { get; }

            public ColumnAssociation(string name, int index)
            {
                this.Name = name ?? throw new ArgumentNullException(nameof(name));
                this.Index = index;
            }
        }

        public override Reflector? Parse(HtmlDocument doc)
        {
            var name = doc.DocumentNode.SelectSingleNode("//title")?.InnerText.Split(' ')[0] ?? String.Empty;

            var table = FindHeardUsersTable(doc);

            if (table == null)
            {
                return null;
            }

            var columnAssociations = ParseColumns(table);
            var rows = table.SelectNodes(".//tr");

            var heardUsers = new List<ReflectorHeardUser>();

            foreach (var row in rows)
            {
                var nodes = row.SelectNodes(".//td");

                if (nodes == null || nodes.Count() != 8)
                {
                    continue;
                }

                var remoteUser = new ReflectorHeardUser();

                var columnValues = nodes.ToArray();
                var callSignColumn = columnValues[columnAssociations[ColumnType.Callsign].Index];
                var heardOnColumn = columnValues[columnAssociations[ColumnType.HeardOn].Index];
                var lastHeardColumn = columnValues[columnAssociations[ColumnType.LastHeard].Index];

                remoteUser.Callsign = callSignColumn.InnerText;
                remoteUser.LastHeard = ParseDate(lastHeardColumn.InnerText);
                remoteUser.HeardOn = heardOnColumn.InnerText;
            }

            var reflector = new Reflector(name, heardUsers);
            return reflector;
        }

        public HtmlNode? FindHeardUsersTable(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Last heard')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Callsign')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Nominativo')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Znak')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'DV Station')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Çağrı İşareti')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Rufzeichen')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//tr/th[contains(.,'Инициал')]");

            if (node != null)
            {
                return node.ParentNode.ParentNode;
            }

            node = doc.DocumentNode.SelectSingleNode("//table//th[contains(.,'MyCall')]");

            if (node != null)
            {
                return node.ParentNode;
            }

            if (!doc.DocumentNode.InnerHtml.Contains("DSTAR dashboard by David PA7LIM")
                && !doc.DocumentNode.InnerHtml.Contains("http://93.186.254.219/db/index.php")
                && !doc.ParsedText.Contains("EASYDNS-FORWARDER")
                && !doc.ParsedText.Contains("http://bh4jbv.site/xlxd/")
                && !doc.ParsedText.Contains("http://xlx.xrf105.fr")
                && !doc.ParsedText.Contains("http://80.211.94.145/214/")
                && !doc.ParsedText.Contains("http://162.238.214.18/xlx256/")
                && !doc.ParsedText.Contains("http://178.254.33.188/")
                && !doc.ParsedText.Contains("http://179.51.0.153:8503/dashboard/index.php")
                && !doc.ParsedText.Contains("https://ke0tcf.radio/Utilities/Reflector")
                && !doc.ParsedText.Contains("http://dv.pd3rfr.nl:8080/xlxd/")
                && !doc.ParsedText.Contains("http://45.32.82.218/db/")
                && !doc.ParsedText.Contains("http://xlx991.iz7auh.net/index.php")
                && !doc.ParsedText.Contains("XLX246 Reflector Dashboard")
                && !doc.ParsedText.Contains("je4smq@jarl.com")
                && !doc.ParsedText.Contains("<meta http-equiv=\"refresh\" content=")
                && !doc.ParsedText.Contains("<meta http-equiv='Refresh'content =")
                && !doc.ParsedText.Contains("LX1IQ and Dashboard modifed by HB9GFX")
                && !doc.ParsedText.Contains("Reflecteur et Serveur Starnet multi protocoles francophone, fourni par F5KAV")
                && !doc.ParsedText.Contains("If you see this page, the nginx web server is successfully installed and")
                && !doc.ParsedText.Contains("This is the default welcome page used to test the correct")
                && !doc.ParsedText.Contains("Welcome to CentOS")
                && !doc.ParsedText.Contains("The requested URL was not found on this server.")
                && !doc.ParsedText.Contains("You don't have permission to access this resource."))
            {
                throw new Exception("Couldn't find heard users table.");
            }

            return null;
        }

        private IDictionary<ColumnType, ColumnAssociation> ParseColumns(HtmlNode table)
        {
            var thNodes = table.SelectNodes(".//th[not(.//table)]");
            var result = new Dictionary<ColumnType, ColumnAssociation>();

            for(int index = 0; index < thNodes.Count(); index++)
            {
                var name = thNodes[index].InnerText;
                if (name == "Callsign" 
                    || name == "MyCall"
                    || name == "Nominativo"
                    || name == "Znak"
                    || name == "DV Station"
                    || name == "Çağrı İşareti"
                    || name == "Rufzeichen"
                    || name == "Инициал")
                {
                    result[ColumnType.Callsign] = new ColumnAssociation(name, index);
                }
                else if (name == "Last heard"
                    || name == "Last heard (UTC)"
                    || name == "Last Heard - UTC"
                    || name == "Last heard EST/EDT"
                    || name == "Last heard (JST)"
                    || name == "Last heard (GMT)"
                    || name == "Last heard (Local)"
                    || name == "Last Heard"
                    || name == "Last TX"
                    || name == "Ascoltato"
                    || name == "Godzina"
                    || name == "Son Duyulma"
                    || name == "Zuletzt gehoert"
                    || name == "Последно чут")
                {
                    result[ColumnType.LastHeard] = new ColumnAssociation(name, index);
                }
                else if (
                    thNodes[index].ChildNodes.FirstOrDefault()?.GetAttributeValue("alt", string.Empty) == "Listening on"
                    || name == "Module"
                    || name == "Group"
                    || name == "Modulo")
                {
                    result[ColumnType.HeardOn] = new ColumnAssociation("Listening on", index);
                }
            }

            if (!(result.Count() == 3
                ||(result.Count() == 2 && result.ContainsKey(ColumnType.HeardOn) == false)))
            {
                throw new Exception("Couldn't find all columns.");
            }

            return result;
        }

        private DateTime ParseDate(string date)
        {
            try
            {
                return DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "MM.dd.yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "MM-dd-yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "yyyy.MM.dd. HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "dd.MM.yyyy - HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            try
            {
                return DateTime.ParseExact(date, "HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            throw new Exception($"Couldn't parse date: '{date}'");
        }
    }
}
