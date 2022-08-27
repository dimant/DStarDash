namespace DStarDash.Models
{
    public class Gateway
    {
        public string Version { get; private set; }

        public IEnumerable<GatewayRemoteUser> RemoteUsers { get; private set; }

        public IEnumerable<GatewayHeardUser> HeardUsers { get; private set; }

        public Gateway(string version, IEnumerable<GatewayRemoteUser> remoteUsers, IEnumerable<GatewayHeardUser> heardUsers)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            RemoteUsers = remoteUsers ?? throw new ArgumentNullException(nameof(remoteUsers));
            HeardUsers = heardUsers ?? throw new ArgumentNullException(nameof(heardUsers));
        }
    }
}
