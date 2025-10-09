using Scrapers.Services;

if (args.Length == 0)
{
    Console.WriteLine("missing args");
    return;
}

if (args.Contains("--ktc"))
{
    var scraper = new KtcScraper();
    await scraper.ScrapeAndSaveAsync();
}

else
{
    Console.WriteLine("invalid arg");
    return;
}
