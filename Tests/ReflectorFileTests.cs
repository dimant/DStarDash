namespace DStarDash.Tests
{
    using Xunit;

    public class ReflectorFileTests
    {
        [Fact]
        public void AppendsHtmlExtension()
        {
            Assert.Equal("XLX801.html", ReflectorFile.NameFor("XLX801"));
        }

        [Fact]
        public void StripsPathSeparatorsToPreventTraversal()
        {
            var name = ReflectorFile.NameFor("../../etc/passwd");

            Assert.DoesNotContain('/', name);
            Assert.DoesNotContain(Path.DirectorySeparatorChar, name);
            Assert.DoesNotContain(Path.AltDirectorySeparatorChar, name);
        }

        [Fact]
        public void ReplacesInvalidFileNameCharacters()
        {
            var name = ReflectorFile.NameFor("A:B*C?");

            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                Assert.DoesNotContain(invalid, name);
            }
        }

        [Fact]
        public void PathForCombinesDirectoryWithSanitizedName()
        {
            Assert.Equal(Path.Combine("data", "XLX801.html"), ReflectorFile.PathFor("data", "XLX801"));
        }

        [Fact]
        public void PathForKeepsReflectorInsideDirectoryDespiteTraversalName()
        {
            var path = ReflectorFile.PathFor("data", "../../etc/passwd");

            Assert.Equal("data", Path.GetDirectoryName(path));
        }
    }
}
