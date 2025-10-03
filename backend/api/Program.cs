using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
