using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using Shared.Services;
using WebScraping.Scrapers;
using Shared.DTOs;
using WebScraping;
using Data.Services;
using Data.Models;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<FcScraper>(
    client =>
    {
        client.BaseAddress = new Uri(Consts.FcBaseUrl);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", Consts.UserAgent);
    }
);

builder.Services.AddTransient<KtcScraper>();
builder.Services.AddTransient<FileService>();
builder.Services.AddTransient<PlayerValueService>();

using IHost host = builder.Build();

try
{
    if (args.Length == 0)
    {
        throw new Exception("missing args");
    }

    if (args[0] == "push")
    {
        PlayerValueService PlayerValueService = host.Services.GetRequiredService<PlayerValueService>();
        var data = fileService.ReadFromFileJson();
        PlayerValueService.ProcessPlayerValueData();
        return;
    }
    else
    {
        string fileName;
        IScraper scraper;
        List<ScrapedPlayer> scraped;
        FileService fileService = host.Services.GetRequiredService<FileService>();

        if (args[0] == Consts.SourceKtc)
        {
            fileName = Consts.SourceKtc;
            scraper = host.Services.GetRequiredService<KtcScraper>();

            scraped = await scraper.ScrapeAsync();
        }
        else if (args[0] == Consts.SourceFc)
        {
            fileName = Consts.SourceFc;
            scraper = host.Services.GetRequiredService<FcScraper>();

            scraped = await scraper.ScrapeAsync();
        }

        else
        {
            throw new Exception("invalid arg");
        }

        fileService.WriteToFileJson(fileName, scraped);
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}