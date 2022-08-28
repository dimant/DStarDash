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

            if (!doc.DocumentNode.InnerHtml.Contains("DSTAR dashboard by David PA7LIM"))
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
                if (name == "Callsign" || name == "MyCall")
                {
                    result[ColumnType.Callsign] = new ColumnAssociation(name, index);
                }
                else if (name == "Last heard" || name == "Last heard (UTC)")
                {
                    result[ColumnType.LastHeard] = new ColumnAssociation(name, index);
                }
                else if (
                    thNodes[index].ChildNodes.FirstOrDefault()?.GetAttributeValue("alt", string.Empty) == "Listening on"
                    || name == "Module")
                {
                    result[ColumnType.HeardOn] = new ColumnAssociation("Listening on", index);
                }
            }

            if (result.Count() != 3)
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
                return DateTime.ParseExact(date, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
            }

            throw new Exception($"Couldn't parse date: '{date}'");
        }
    }
}
