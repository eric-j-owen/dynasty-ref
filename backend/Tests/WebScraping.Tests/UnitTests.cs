using WebScraping.Utilities;

namespace WebScraping.Tests;

public class UnitTests
{
    [Theory]
    [InlineData("Tom Brady", "tombrady")]
    [InlineData("Ja'Marr Chase","jamarrchase")]
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
