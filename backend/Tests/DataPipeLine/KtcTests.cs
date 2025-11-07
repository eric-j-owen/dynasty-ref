using DataPipeline.DataProviders;
using DataPipeline.DTOs;
using HtmlAgilityPack;

namespace Tests.DataPipeline;

public class KtcTests
{
    [Fact]
    public void ParsePlayersFromDocument_ReturnsParsedPlayerList()
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        var testFilePath = Path.Combine(projectRoot, "TestData", "KtcPage.html");

        var html = new HtmlDocument();
        html.Load(testFilePath);

        var actual = KtcValuesScraper.ParsePlayersFromDocument(html);

        var expected = new KtcScrapedPlayerDto
        {
            KtcId = 365,
            OneQbValues = new KtcValueData { Value = 8147 },
            SuperFlexValues = new KtcValueData { Value = 9992 }
        };


        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(4, actual.Count);
        Assert.Equal(expected.KtcId, actual.First().KtcId);
        Assert.Equal(expected.OneQbValues.Value, actual.First().OneQbValues.Value);
        Assert.Equal(expected.SuperFlexValues.Value, actual.First().SuperFlexValues.Value);
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