namespace DStarDash.Models
{
    public class GatewayHeardUser
    {
        public string Callsign { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string HeardOn { get; set; } = string.Empty;

        public DateTime HeardAt { get; set; }
    }
}
