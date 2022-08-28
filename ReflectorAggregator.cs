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
                var name = reflectors[key].First().Name;

                var path = $"{name}.html";

                try
                {
                    downloader.DownloadFileAsync(key, path).Wait();
                }
                catch (Exception /* e */)
                {
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
                if (string.IsNullOrEmpty(module.Url))
                {
                    continue;
                }

                if (reflectors.ContainsKey(module.Url))
                {
                    reflectors[module.Url].Add(module);
                }
                else
                {
                    reflectors[module.Url] = new List<ReflectorModule>();
                    reflectors[module.Url].Add(module);
                }
            }

            return reflectors;
        }
    }
}
