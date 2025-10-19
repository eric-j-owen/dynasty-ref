using WebScraping.Helpers;

namespace WebScraping.Tests
{
    public class HelpersUnitTests
    {
        [Theory]
        [InlineData("Tom Brady", "tombrady")]
        [InlineData("Ja'Marr Chase", "jamarrchase")]
        [InlineData("A.J. Brown", "ajbrown")]
        [InlineData("Derrick Kelly II", "derrickkellyii")]
        [InlineData("", "")]
        public void Name_IsNormalized(string input, string want)
        {
            var got = NormalizeField.Name(input);
            Assert.Equal(want, got);
        }

        [Theory]
        [InlineData("RB5", "RB")]
        [InlineData("TE13", "TE")]
        public void Position_IsNormalized(string input, string want)
        {
            var got = NormalizeField.Position(input);
            Assert.Equal(want, got);
        }
    }

    public class ScraperUnitTests
    {
        // [Theory]
        // public void ScrapePlayer_ReturnsPlayerObject()
        // {
            
        // }
    }
}