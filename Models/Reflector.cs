namespace DStarDash.Models
{
    public class Reflector
    {
        public string Name { get; private set; }

        public IEnumerable<ReflectorHeardUser> HeardUsers { get; private set; }

        public Reflector(
            string name,
            IEnumerable<ReflectorHeardUser> heardUsers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            HeardUsers = heardUsers ?? throw new ArgumentNullException(nameof(heardUsers));
        }
    }
}
