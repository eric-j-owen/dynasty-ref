using DataPipeline.DataProviders;
using HtmlAgilityPack;

namespace Tests.Unit;

public class KtcTests
{
    [Fact]
    public void ParsePlayersFromDocument_ReturnsParsedPlayerList()
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
           .Parent!.Parent!.Parent!.FullName;

        var testFilePath = Path.Combine(projectRoot, "TestData", "KtcPage.html");

        var html = new HtmlDocument();
        html.Load(testFilePath);

        var actual = KtcProvider.ParsePlayersFromDocument(html);

        var expected = new[]
        {
            new {name = "Josh Allen"},
            new {name = "Bijan Robinson"},
            new {name = "Ja'Marr Chase"},
            new {name = "Puka Nacua"},
        };

        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(4, actual.Count);
        Assert.Equal(expected[0].name, actual[0].PlayerName);
        Assert.Equal(expected[1].name, actual[1].PlayerName);
        Assert.Equal(expected[2].name, actual[2].PlayerName);
        Assert.Equal(expected[3].name, actual[3].PlayerName);
    }
}