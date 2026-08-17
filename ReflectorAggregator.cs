namespace DStarDash
{
    using DStarDash.Parsers;
    using DStarDash.Models;

    public class ReflectorAggregator
    {
        public string ReflectorsPath { get; }

        public string ReflectorsUrl { get; }

        private IReflectorListHtmlParser reflectorListHtmlParser;

        public ReflectorAggregator(
            string reflectorsPath, 
            string reflectorsUrl,
            IReflectorListHtmlParser reflectorListHtmlParser)
        {
            this.ReflectorsPath = reflectorsPath ?? throw new ArgumentNullException(nameof(reflectorsPath));
            this.ReflectorsUrl = reflectorsUrl ?? throw new ArgumentNullException(nameof(reflectorsUrl));
            this.reflectorListHtmlParser = reflectorListHtmlParser ?? throw new ArgumentNullException(nameof(reflectorListHtmlParser));
        }

        public string DataDirectory => Path.GetDirectoryName(this.ReflectorsPath) ?? string.Empty;

        public void DownloadReflectorData(Action<int, int>? progress = null)
        {
            var downloader = new HttpDownloader();

            if (!string.IsNullOrEmpty(this.DataDirectory))
            {
                Directory.CreateDirectory(this.DataDirectory);
            }

            downloader.DownloadFileAsync(this.ReflectorsUrl, this.ReflectorsPath).Wait();

            var reflectors = this.ReflectorsFromFile(this.ReflectorsPath);

            var failures = this.DownloadReflectors(reflectors, downloader, progress);

            if (failures.Count > 0)
            {
                Console.Error.WriteLine(
                    $"Failed to download {failures.Count} of {reflectors.Count} reflectors: {string.Join(", ", failures)}");
            }
        }

        /// <summary>
        /// Downloads each reflector's dashboard in parallel, returning the names of
        /// reflectors whose download threw. Separated from <see cref="DownloadReflectorData"/>
        /// so failure handling is testable without hitting the network.
        /// </summary>
        public IReadOnlyList<string> DownloadReflectors(
            IDictionary<string, List<ReflectorModule>> reflectors,
            IFileDownloader downloader,
            Action<int, int>? progress)
        {
            int n = reflectors.Count;
            int completed = 0;
            var failures = new System.Collections.Concurrent.ConcurrentBag<string>();

            Parallel.ForEach(reflectors.Keys, (key) =>
            {
                var name = reflectors[key].First().Name;

                var path = ReflectorFile.PathFor(this.DataDirectory, name);

                try
                {
                    downloader.DownloadFileAsync(key, path).Wait();
                }
                catch (Exception /* e */)
                {
                    failures.Add(name);
                }

                int done = Interlocked.Increment(ref completed);
                progress?.Invoke(done, n);
            });

            return failures.ToList();
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
