using WebScraping.Helpers;
using WebScraping.Scrapers;

namespace WebScraping.Tests
{
    public class HelpersUnitTests
    {
        [Theory]
        [InlineData("Ja&#x27;Marr Chase", "jamarrchase")]
        [InlineData("A.J. Brown", "ajbrown")]
        [InlineData("De&#x27;Von Achane", "devonachane")]
        public void Name_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Name(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("RB5", "RB")]
        [InlineData("TE13", "TE")]
        public void Position_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Position(input);
            Assert.Equal(expected, actual);
        }
    }

    public class ScraperUnitTests
    {
        private readonly KtcScraper _ktc;
        public ScraperUnitTests()
        {
            _ktc = new KtcScraper();
        }
        
        [Fact]
        public void ParsePlayer_ReturnsPlayerObject()
        {
            string name = "Brian Thomas Jr.";
            int value = 5000;
            string team = "JAC";
            string position = "WR13";

            var actual = KtcScraper.ParsePlayer(name, value, team, position);

            Assert.NotNull(actual);
            Assert.Equal("brianthomasjr", actual.SearchFullName);
            Assert.Equal(5000, actual.Value);
            Assert.Equal("JAX", actual.Team);
            Assert.Equal("WR", actual.Position);
        }
    }
}