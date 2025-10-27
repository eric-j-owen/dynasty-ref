using DataPipeline.Interfaces;
using DataPipeline.DataProviders;

namespace DataPipeline.Services;

public class KtcService : IDataPipelineService
{
    private readonly KtcScraper _scraper;

    public KtcService(KtcScraper scraper)
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