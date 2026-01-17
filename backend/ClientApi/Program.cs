using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Db;
using Shared.Consts;
using System.Text.Json.Serialization;
using ClientApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://dynasty-ref.xyz");
        });
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 60,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("rate limited", cancellationToken);
    };
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<EspnService>(client =>
{
    client.BaseAddress = new Uri(ApiBaseUrl.Espn);
});

builder.Services.AddScoped<PlayerService>();

builder.Services.AddControllers()
   .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.UseRateLimiter();
app.UsePathBase("/api");
app.MapControllers();

app.Run();
