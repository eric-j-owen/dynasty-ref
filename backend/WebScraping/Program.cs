using WebScraping.Services;
using WebScraping.Scrapers;

try
{
    if (args.Length == 0)
    {
        throw new Exception("missing args");
    }
    else
    {
        if (args.Contains("--ktc"))
        {
            var scraper = new KtcScraper();
            await scraper.ScrapeAndSaveAsync();
        }
        else if (args.Contains("--fc"))
        {
            var scraper = new FcScraper();
            await scraper.ScrapeAndSaveAsync();
        }

        else if (args.Contains("--push"))
        {
            DbService.ProcessData();
        }

        else
        {
            throw new Exception("invalid arg");
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e); 
}