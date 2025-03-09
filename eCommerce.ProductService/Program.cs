using eCommerce.ProductService.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
    serverOptions.ConfigureEndpointDefaults(options =>
    {
        options.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });

});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    // This will ensure the response is in JSON format.
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

        // Set the response content type to application/json
        context.Response.ContentType = "application/json";

        // Write the JSON response
        await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
    }
});

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }

