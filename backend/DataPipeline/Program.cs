using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DataPipeline.Services;
using Db;
using DataPipeline.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataPipeline.DataProviders;
using Shared.Consts;
using DataPipeline.DataProviders.PlayerMaster;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("")));

builder.Services.AddTransient<PlayerMasterService>();
builder.Services.AddTransient<KtcService>();

//data providers
builder.Services.AddHttpClient<ExtractSleeperPlayers>(client =>
    {
        client.BaseAddress = new Uri(Api.BaseUrls.Sleeper);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
);

builder.Services.AddHttpClient<KtcScraper>(client =>
    {
        client.BaseAddress = new Uri(Api.BaseUrls.Ktc);
        client.DefaultRequestHeaders.Add("User-Agent", Api.Config.UserAgent);
    }

);


using IHost host = builder.Build();

try
{
    if (args.Length == 0)
    {
        throw new Exception("missing args");
    }

    var arg = args[0];
    IDataPipelineService service = arg switch
    {
        "players" => host.Services.GetRequiredService<PlayerMasterService>(),
        "ktc" => host.Services.GetRequiredService<KtcService>(),
        _ => throw new Exception("invalid argument"),
    };

    await service.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

