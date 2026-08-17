namespace DStarDash.Tests
{
    using DStarDash;
    using DStarDash.Models;
    using Xunit;

    public class ProgramSortTests
    {
        private static StatsRow Row(string name, int heard, DateTime last, Status status = Status.OK)
            => new StatsRow(name, "loc", status, heard, last, "");

        private static List<StatsRow> Sample() => new()
        {
            Row("Charlie", 5, new DateTime(2022, 1, 1)),
            Row("Alpha", 20, new DateTime(2022, 3, 1)),
            Row("Bravo", 12, new DateTime(2022, 2, 1)),
        };

        [Fact]
        public void SortsByHeardDescending()
        {
            var sorted = Program.Sort(Sample(), "Heard");

            Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, sorted.Select(r => r.Name));
        }

        [Fact]
        public void SortsByLastHeardDescending()
        {
            var sorted = Program.Sort(Sample(), "Last");

            Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, sorted.Select(r => r.Name));
        }

        [Fact]
        public void SortsByNameForUnknownKey()
        {
            var sorted = Program.Sort(Sample(), "somethingelse");

            Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, sorted.Select(r => r.Name));
        }

        [Fact]
        public void EmptySortByLeavesOrderUnchanged()
        {
            var sorted = Program.Sort(Sample(), "");

            Assert.Equal(new[] { "Charlie", "Alpha", "Bravo" }, sorted.Select(r => r.Name));
        }
    }
}
