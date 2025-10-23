using DataPipeline.Transformers;
using DataPipeline.DataProviders;
using HtmlAgilityPack;

namespace Tests.Unit;

public class TestingKtcProvider : KtcProvider
{
    protected override HtmlDocument LoadHtml(string path)
    {
        var doc = new HtmlDocument();
        doc.Load(path);
        return doc;
    }
}

public class KtcTests
{

    private readonly string _testFilePath;
    public KtcTests()
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
           .Parent!.Parent!.Parent!.FullName;

        _testFilePath = Path.Combine(projectRoot, "TestData", "KtcPage.html");
    }

    [Fact]
    public async Task ExtractDataAsync_ReturnsPlayerList()
    {

        var provider = new TestingKtcProvider();
        var actual = await provider.ExtractDataAsync(_testFilePath);

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

    [Fact]
    public void ParsePlayer_ReturnsCorrectPlayerObject()
    {
        string name = "Brian Thomas Jr.";
        int value = 5000;
        string team = "JAC";
        string position = "WR13";

        var actual = KtcTransform.ParsePlayer(name, value, team, position);

        Assert.NotNull(actual);
        Assert.Equal("brianthomasjr", actual.SearchFullName);
        Assert.Equal(5000, actual.Value);
        Assert.Equal("JAX", actual.Team);
        Assert.Equal("WR", actual.Position);
    }
}