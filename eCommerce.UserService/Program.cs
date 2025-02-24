using System.Diagnostics;
using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var basePath = AppDomain.CurrentDomain.BaseDirectory;
var dbPath = Path.Combine(basePath, "UserServiceDb.sqlite");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

Console.WriteLine(dbPath);


builder.Services.AddGrpc();
builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.MapGrpcService<UserService>();


app.Run();