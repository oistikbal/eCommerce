using System.Text;
using eCommerce.Gateway.Middleware;
using eCommerce.UserService.Protos.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
    serverOptions.ConfigureEndpointDefaults(options =>
    {
        options.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});


builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("https://localhost:5000/health"), name: "eCommerce.UserService");

builder.Services.AddHealthChecksUI(setupSettings: setup =>
{
    setup.AddHealthCheckEndpoint("eCommerce.Gateway", "https://localhost:3000/health");
    setup.AddHealthCheckEndpoint("eCommerce.UserService", "https://localhost:5000/health");
}).AddInMemoryStorage();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001");
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new InvalidOperationException("JWT settings (SecretKey, Issuer, Audience) are required.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Default", policy =>
        {
            policy.RequireAuthenticatedUser();
        }
)   ;

    options.AddPolicy("Administrator", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.Requirements.Add(new RoleRequirement("Admin"));
        }
    );
});

builder.Services.AddSingleton<IAuthorizationHandler, RoleBasedHandler>();


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.UseHealthChecks("/health", new HealthCheckOptions
{

    ResponseWriter = async (context, report) =>
    {
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
    }
});
app.UseHealthChecksUI(options =>
{
    options.PageTitle = "eCommerce";
    options.UIPath = "/health-ui";
    options.AddCustomStylesheet("healthui.css");
});

app.MapHealthChecks("/health");



if (app.Environment.IsDevelopment())
{
    // Skip HTTPS requirement in development
    app.UseHsts();
}
else
{
    // Force HTTPS in production
    app.UseHttpsRedirection();
}

app.Run();
