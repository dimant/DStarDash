namespace DStarDash.Models
{
    public class ReflectorHeardUser
    {
        public string Callsign { get; set; } = string.Empty;

        public string HeardOn { get; set; } = string.Empty;

        public DateTime LastHeard { get; set; }
    }
}
