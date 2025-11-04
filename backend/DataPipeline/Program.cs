using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
// using System.Net.Http.Headers;
using Db;
using Shared.Consts;
using DataPipeline.Loaders;
using DataPipeline.DataProviders;
using DataPipeline.Pipelines;
using DataPipeline.Interfaces;
using DataPipeline.DataTransformers;
using DataPipeline.DTOs;
using Db.Models;
// using DataPipeline.Services;
// using DataPipeline.Interfaces;
// using DataPipeline.DataProviders;
// using Shared.Consts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);


builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));





// builder.Services.AddHttpClient<GetDynastyProcessPlayers>(client =>
// {
//     client.BaseAddress = new Uri(ApiConsts.BaseUrl.DynastyProcess);
//     client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
//     client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.raw+json");
//     client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "");

// });

// data providers
builder.Services.AddHttpClient<IDataProvider<SleeperPlayer>, GetSleeperPlayers>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Sleeper);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

//transformers
builder.Services.AddTransient<IDataTransformer<SleeperPlayer>, SleeperPlayerTransformer>();

//loaders
builder.Services.AddTransient<IDataLoader<Player>, PlayerUpsertLoader>();

//pipelines
builder.Services.AddTransient<PlayerPipeline>();


// builder.Services.AddHttpClient<KtcValuesScraper>(client =>
// {
//     client.BaseAddress = new Uri(ApiConsts.BaseUrl.Ktc);
//     client.DefaultRequestHeaders.Add("User-Agent", ApiConsts.Config.UserAgent);
// });


using IHost host = builder.Build();

try
{
    if (args.Length == 0)
    {
        throw new Exception("missing args");
    }

    var arg = args[0];
    IPipeline service = arg switch
    {
        "players" => host.Services.GetRequiredService<PlayerPipeline>(),
        _ => throw new Exception("invalid argument"),
    };

    await service.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

