using DataPipeline.DataProviders;
using HtmlAgilityPack;

namespace Tests.DataPipeline;

public class KtcPipelineTests
{
    [Fact]
    public void ParsePlayersFromDocument_ReturnsParsedPlayerList()
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        var testFilePath = Path.Combine(projectRoot, "TestData", "KtcPage.html");

        var html = new HtmlDocument();
        html.Load(testFilePath);

        var actual = KtcValuesScraper.ParsePlayersFromDocument(html);

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
        for (var i = 0; i < actual.Count; i++)
        {
            Assert.Equal(expected[i].name, actual[i].PlayerName);
        }
    }

    [Fact]
    public void ParsePlayersFromDocument_ThrowsError_NullScriptTags()
    {
        var html = "<html><head></head></html>";
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        Assert.Throws<Exception>(() => KtcValuesScraper.ParsePlayersFromDocument(doc));
    }

    [Fact]
    public void ParsePlayersFromDocument_ThrowsError_WhenEmptyData()
    {
        var html = "<html><head><script></script></head></html>";
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        Assert.Throws<Exception>(() => KtcValuesScraper.ParsePlayersFromDocument(doc));
    }

}