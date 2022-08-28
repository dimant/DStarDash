namespace DStarDash.Models
{
    public class Reflector
    {
        public string Name { get; private set; }

        public IEnumerable<ReflectorHeardUser> HeardUsers { get; private set; }

        public Reflector(
            string name,
            string version,
            Dictionary<string, List<string>> linkedGateways,
            IEnumerable<ReflectorRemoteUser> remoteUsers,
            IEnumerable<ReflectorHeardUser> heardUsers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            HeardUsers = heardUsers ?? throw new ArgumentNullException(nameof(heardUsers));
        }
    }
}
