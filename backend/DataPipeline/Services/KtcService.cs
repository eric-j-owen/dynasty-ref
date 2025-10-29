using DataPipeline.Interfaces;
using DataPipeline.DataProviders;

namespace DataPipeline.Services;

public class KtcService : IDataPipelineService
{
    private readonly KtcRankingsScraper _scraper;

    public KtcService(KtcRankingsScraper scraper)
    {
        _scraper = scraper;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("ktc service start");
        var data = await _scraper.ExtractDataAsync();
        Console.WriteLine("ktc service complete");
    }
}