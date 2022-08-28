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

        public Status Status { get; }

        public int LinkedGateways { get; }

        public int RemoteUsers { get; }

        public int HeardUsers { get; }

        public DateTime LastHeard { get; }

        public StatsRow(string name, Status status, int linkedGateways, int remoteUsers, int heardUsers, DateTime lastHeard)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Status = status;
            LinkedGateways = linkedGateways;
            RemoteUsers = remoteUsers;
            HeardUsers = heardUsers;
            LastHeard = lastHeard;
        }
    }
}
