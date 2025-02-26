using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Services.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<UserServiceV1>();
app.Run();

public partial class Program { }