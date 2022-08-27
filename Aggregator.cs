namespace DStarDash
{
    using DStarDash.Models;

    public class ReflectorAggregator
    {
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
