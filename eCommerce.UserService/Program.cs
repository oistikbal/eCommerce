using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddGrpc();
builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.MapGrpcService<UserService>();


app.Run();

public partial class Program { }