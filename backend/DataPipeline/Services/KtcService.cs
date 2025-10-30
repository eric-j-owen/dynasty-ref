using DataPipeline.Interfaces;
using DataPipeline.DataProviders;

namespace DataPipeline.Services;

public class KtcService
{
    private readonly KtcValuesScraper _scraper;

    public KtcService(KtcValuesScraper scraper)
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