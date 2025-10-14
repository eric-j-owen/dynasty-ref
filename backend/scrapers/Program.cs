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
else if (args.Contains("--fc"))
{
    var scraper = new FcScraper();
    await scraper.ScrapeAndSaveAsync();
}

else if (args.Contains("--push"))
{
    var db = new DbService();
    db.Main();
}

else
{
    Console.WriteLine("invalid arg");
}
