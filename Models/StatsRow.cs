namespace DStarDash.Models
{
    public enum Status
    {
        OK,
        Fail
    }

    public class StatsRow
    {
        public string Name { get; }

        public string Location { get; }

        public Status Status { get; }

        public int LinkedGateways { get; }

        public int RemoteUsers { get; }

        public int HeardUsers { get; }

        public DateTime LastHeard { get; }

        public string BusiestModule { get; }

        public StatsRow(
            string name,
            string location,
            Status status,
            int linkedGateways,
            int remoteUsers,
            int heardUsers,
            DateTime lastHeard,
            string busiestModule)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Location = location ?? throw new ArgumentException(nameof(location));
            Status = status;
            LinkedGateways = linkedGateways;
            RemoteUsers = remoteUsers;
            HeardUsers = heardUsers;
            LastHeard = lastHeard;
            BusiestModule = busiestModule;
        }
    }
}
