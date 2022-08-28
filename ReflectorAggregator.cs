namespace DStarDash
{
    using DStarDash.Parsers;
    using DStarDash.Models;

    public class ReflectorAggregator
    {
        public string ReflectorsPath { get; }

        private IReflectorListHtmlParser reflectorListHtmlParser;

        public ReflectorAggregator(
            string reflectorsPath, 
            IReflectorListHtmlParser reflectorListHtmlParser)
        {
            this.ReflectorsPath = reflectorsPath ?? throw new ArgumentNullException(nameof(reflectorsPath));
            this.reflectorListHtmlParser = reflectorListHtmlParser ?? throw new ArgumentNullException(nameof(reflectorListHtmlParser));
        }

        public void DownloadReflectorData(Action<int, int>? progress = null)
        {
            var downloader = new HttpDownloader();

            downloader.DownloadFileAsync("http://apps.dstarinfo.com/reflectors.aspx", this.ReflectorsPath).Wait();

            var reflectors = this.ReflectorsFromFile(this.ReflectorsPath);

            int n = reflectors.Keys.Count();
            int i = 0;

            Parallel.ForEach(reflectors.Keys, (key) =>
            {
                if (key.Contains(".dstargateway.org"))
                {
                    var path = ReflectorPathFromUrl(key);

                    try
                    {
                        downloader.DownloadFileAsync(key, path).Wait();
                    }
                    catch (Exception /* e */)
                    {
                    }
                }

                if (progress != null)
                {
                    progress(i, n);
                }

                i++;
            });
        }

        public static string ReflectorNameFromUrl(string url)
        {
            var uri = new Uri(url);
            var refname = uri.Host.Replace(".", "_");

            return refname;
        }

        public static string ReflectorPathFromUrl(string url)
        {
            var refname = ReflectorNameFromUrl(url);
            var path = $"{refname}.html";

            return path;
        }

        public IDictionary<string, List<ReflectorModule>> ReflectorsFromFile(string path)
        {
            var modules = this.reflectorListHtmlParser.ParseFromFile(path);

            return Reflectors(modules);
        }

        public IDictionary<string, List<ReflectorModule>> ReflectorsFromUrl(string url)
        {
            var modules = this.reflectorListHtmlParser.ParseFromUrl(url);

            return Reflectors(modules);
        }

        public IDictionary<string, List<ReflectorModule>> Reflectors(IEnumerable<ReflectorModule> modules)
        {
            var reflectors = new Dictionary<string, List<ReflectorModule>>();

            foreach (var module in modules)
            {
                if (string.IsNullOrEmpty(module.Status))
                {
                    continue;
                }

                if (reflectors.ContainsKey(module.Status))
                {
                    reflectors[module.Status].Add(module);
                }
                else
                {
                    reflectors[module.Status] = new List<ReflectorModule>();
                    reflectors[module.Status].Add(module);
                }
            }

            return reflectors;
        }
    }
}
