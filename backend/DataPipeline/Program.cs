using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Db;
using Shared.Consts;
using DataPipeline.Interfaces;
using DataPipeline.DataPipeline.Extract;
using DataPipeline.Pipelines;
using DataPipeline.DataPipeline.Extract.PlayerValueSources;
using DataPipeline.DataPipeline.Transform;
using DataPipeline.DataPipeline.Load;



/*
==================
    config 
==================
*/
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql
    (
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o
            .MapEnum<DataSource>("data_source")
            .MapEnum<TeamAbbr>("team")
            .MapEnum<IncludedPosition>("included_pos")
    ));

/*
==================
    services 
==================
*/

// data providers
builder.Services.AddTransient<PlayerValuesExtract>();
builder.Services.AddHttpClient<SleeperPlayersExtract>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Sleeper);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ExternalIdsExtract>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Github);
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.raw+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    client.DefaultRequestHeaders.Add("User-Agent", "dynasty-ref");
    client.DefaultRequestHeaders.Add
        ("Authorization", builder.Configuration["ApiKeys:Github"]);
});

builder.Services.AddHttpClient<KtcValuesExtract>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Ktc);
    client.DefaultRequestHeaders.Add("User-Agent", ApiConfig.UserAgent);
});

builder.Services.AddHttpClient<FcValuesExtract>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Fc);
    client.DefaultRequestHeaders.Add("User-Agent", ApiConfig.UserAgent);
});

//transformers 
builder.Services.AddTransient<SleeperPlayerTransformer>();
builder.Services.AddTransient<ExternalIdTransform>();

//loaders
builder.Services.AddTransient<PlayerUpsertLoader>();
builder.Services.AddTransient<ExternalIdsLoader>();
builder.Services.AddTransient<PlayerValuesLoader>();


//pipelines
builder.Services.AddTransient<PlayerPipeline>();
builder.Services.AddTransient<ExternalIdsPipeline>();
builder.Services.AddTransient<PlayerValuesPipeline>();


using IHost host = builder.Build();


/*
==================
    control flow 
==================
*/
try
{
    if (args.Length == 0)
    {
        throw new Exception("missing args");
    }

    var arg = args[0];
    IPipeline pipeline = arg switch
    {
        "players" => host.Services.GetRequiredService<PlayerPipeline>(),
        "ids" => host.Services.GetRequiredService<ExternalIdsPipeline>(),
        "values" => host.Services.GetRequiredService<PlayerValuesPipeline>(),
        _ => throw new Exception("invalid argument"),
    };

    await pipeline.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

