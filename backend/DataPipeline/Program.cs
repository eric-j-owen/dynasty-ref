using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Db;
using Db.Models;
using Shared.Consts;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using DataPipeline.DataPipeline.Extract;
using DataPipeline.DataPipeline.Transform;
using DataPipeline.DataPipeline.Load;
using DataPipeline.Pipelines;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);


builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// data providers
builder.Services.AddHttpClient<IDataProvider<SleeperPlayerDto>, GetSleeperPlayers>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Sleeper);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IDataProvider<DynastyProcessIdsDto>, GetDynastyProcessIds>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Github);
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.raw+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    client.DefaultRequestHeaders.Add("User-Agent", "dynasty-ref");
    client.DefaultRequestHeaders.Add
        ("Authorization", builder.Configuration["ApiKeys:Github"]);
});

// builder.Services.AddHttpClient<KtcValuesScraper>(client =>
// {
//     client.BaseAddress = new Uri(ApiConsts.BaseUrl.Ktc);
//     client.DefaultRequestHeaders.Add("User-Agent", ApiConsts.Config.UserAgent);
// });

//transformers
builder.Services.AddTransient<IDataTransformer<SleeperPlayerDto>, SleeperPlayerTransformer>();
builder.Services.AddTransient<IDataTransformer<DynastyProcessIdsDto>, ExternalIdTransform>();

//loaders
builder.Services.AddTransient<IDataLoader<PlayerModel>, PlayerUpsertLoader>();
builder.Services.AddTransient<IDataLoader<ExternalIdModel>, ExternalIdsLoader>();

//pipelines
builder.Services.AddTransient<RunPipeline<SleeperPlayerDto>>();
builder.Services.AddTransient<RunPipeline<DynastyProcessIdsDto>>();


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
        "players" => host.Services.GetRequiredService<RunPipeline<SleeperPlayerDto>>(),
        "ids" => host.Services.GetRequiredService<RunPipeline<DynastyProcessIdsDto>>(),
        _ => throw new Exception("invalid argument"),
    };

    await service.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

