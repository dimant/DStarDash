namespace DStarDash.Models
{
    public class Reflector
    {
        public string Name { get; private set; }

        public string Version { get; private set; }

        public Dictionary<string, List<string>> LinkedGateways { get; private set; }

        public IEnumerable<ReflectorRemoteUser> RemoteUsers { get; private set; }

        public IEnumerable<ReflectorHeardUser> HeardUsers { get; private set; }

        public Reflector(
            string name,
            string version,
            Dictionary<string, List<string>> linkedGateways,
            IEnumerable<ReflectorRemoteUser> remoteUsers,
            IEnumerable<ReflectorHeardUser> heardUsers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            LinkedGateways = linkedGateways ?? throw new ArgumentNullException(nameof(linkedGateways));
            RemoteUsers = remoteUsers ?? throw new ArgumentNullException(nameof(remoteUsers));
            HeardUsers = heardUsers ?? throw new ArgumentNullException(nameof(heardUsers));
        }
    }
}
