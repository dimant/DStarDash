namespace DStarDash.Parsers
{
    using System.Globalization;
    using System.Text.Json;

    /// <summary>
    /// The data that makes XLX dashboards parseable: the (multilingual) column
    /// header names, the accepted date formats, and the markers identifying pages
    /// that are known not to be dashboards. Historically these were hardcoded in
    /// <see cref="XlxHtmlParser"/>, so every new reflector variant meant a recompile.
    /// They now live here and can be overridden at runtime from a JSON file
    /// (<see cref="Load"/>) — edit data, not code.
    /// </summary>
    public class XlxParsingConfig
    {
        public string[] CallsignHeaders { get; set; } =
        {
            "Callsign", "MyCall", "Nominativo", "Znak", "DV Station",
            "Çağrı İşareti", "Rufzeichen", "Инициал",
        };

        public string[] LastHeardHeaders { get; set; } =
        {
            "Last heard", "Last heard (UTC)", "Last Heard - UTC", "Last heard EST/EDT",
            "Last heard (JST)", "Last heard (GMT)", "Last heard (Local)", "Last Heard",
            "Last TX", "Ascoltato", "Godzina", "Son Duyulma", "Zuletzt gehoert",
            "Последно чут",
        };

        public string[] ModuleHeaders { get; set; } =
        {
            "Module", "Group", "Modulo",
        };

        public string[] DateFormats { get; set; } =
        {
            "dd.MM.yyyy HH:mm",
            "MM.dd.yyyy HH:mm",
            "MM-dd-yyyy HH:mm",
            "yyyy.MM.dd HH:mm",
            "yyyy.MM.dd. HH:mm:ss",
            "yyyy.MM.dd HH:mm:ss",
            "dd.MM.yyyy - HH:mm",
            "yyyy-MM-dd HH:mm",
            "MM/dd/yyyy HH:mm:ss",
            "MMMM d, yyyy HH:mm",
            "HH:mm",
        };

        public string[] NonDashboardMarkers { get; set; } =
        {
            "DSTAR dashboard by David PA7LIM",
            "http://93.186.254.219/db/index.php",
            "EASYDNS-FORWARDER",
            "http://bh4jbv.site/xlxd/",
            "http://xlx.xrf105.fr",
            "http://80.211.94.145/214/",
            "http://162.238.214.18/xlx256/",
            "http://178.254.33.188/",
            "http://179.51.0.153:8503/dashboard/index.php",
            "https://ke0tcf.radio/Utilities/Reflector",
            "http://dv.pd3rfr.nl:8080/xlxd/",
            "http://45.32.82.218/db/",
            "http://xlx991.iz7auh.net/index.php",
            "XLX246 Reflector Dashboard",
            "je4smq@jarl.com",
            "<meta http-equiv=\"refresh\" content=",
            "<meta http-equiv='Refresh'content =",
            "LX1IQ and Dashboard modifed by HB9GFX",
            "Reflecteur et Serveur Starnet multi protocoles francophone, fourni par F5KAV",
            "If you see this page, the nginx web server is successfully installed and",
            "This is the default welcome page used to test the correct",
            "Welcome to CentOS",
            "The requested URL was not found on this server.",
            "You don't have permission to access this resource.",
            "FreeSTAR Network Official XLX Multimode Reflector",
        };

        public static XlxParsingConfig Load(string path)
        {
            if (!File.Exists(path))
            {
                return new XlxParsingConfig();
            }

            return JsonSerializer.Deserialize<XlxParsingConfig>(File.ReadAllText(path))
                ?? new XlxParsingConfig();
        }

        public DateTime ParseDate(string date)
        {
            if (DateTime.TryParseExact(
                    date?.Trim(),
                    this.DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                return parsed;
            }

            throw new FormatException($"Couldn't parse date: '{date}'");
        }
    }
}
