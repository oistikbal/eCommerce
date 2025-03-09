using eCommerce.ProductService.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();


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

