namespace DStarDash
{
    using DStarDash.Models;

    public class ReflectorAggregator
    {
        public void DownloadReflectorData(Action<int, int>? progress = null)
        {
            var downloader = new HttpDownloader();
            var reflectors = this.ReflectorsFromUrl("http://apps.dstarinfo.com/reflectors.aspx");

            int n = reflectors.Keys.Count();
            int i = 0;

            Parallel.ForEach(reflectors.Keys, (key) =>
            {
                if (key.Contains(".dstargateway.org"))
                {
                    var uri = new Uri(key);
                    var segments = uri.Host.Split(".");
                    var refname = segments[0];
                    var path = $"{refname}.html";

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

        public IDictionary<string, List<ReflectorModule>> ReflectorsFromFile(string path)
        {
            var parser = new ReflectorHtmlParser();

            var modules = parser.ParseModulesFromFile(path);

            return Reflectors(modules);
        }

        public IDictionary<string, List<ReflectorModule>> ReflectorsFromUrl(string url)
        {
            var parser = new ReflectorHtmlParser();

            var modules = parser.ParseModulesFromUrl(url);

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
