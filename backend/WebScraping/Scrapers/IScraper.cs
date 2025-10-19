using Shared.Models;

namespace WebScraping.Scrapers;
interface IScraper
{
    Task<List<ScrapedPlayer>> ScrapeAsync();
}