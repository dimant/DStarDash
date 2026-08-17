namespace DStarDash
{
    /// <summary>
    /// Computes the local cache filename for a reflector's downloaded dashboard.
    /// The name is scraped from remote pages, so it is sanitized to prevent path
    /// traversal and invalid-filename errors. Download and summarize paths must
    /// agree, so both go through here.
    /// </summary>
    public static class ReflectorFile
    {
        public static string NameFor(string reflectorName)
        {
            var invalid = Path.GetInvalidFileNameChars();

            var safe = new string((reflectorName ?? string.Empty)
                .Select(c => invalid.Contains(c) ? '_' : c)
                .ToArray());

            return safe + ".html";
        }
    }
}
