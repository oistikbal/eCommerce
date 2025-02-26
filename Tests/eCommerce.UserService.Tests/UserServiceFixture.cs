using eCommerce.UserService.Data;
using eCommerce.UserService.Services.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests
{
    public class UserServiceFixture : WebApplicationFactory<Program>
    {
        private readonly string _dbName;

        public UserServiceFixture()
        {
            _dbName = $"TestDb_{Guid.NewGuid().ToString("N")}.sqlite";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<UserServiceV1>();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite($"Data Source={_dbName}");
                });

                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.EnsureCreated();
                }
            });
        }

        ~UserServiceFixture()
        {
            if (File.Exists(_dbName))
            {
                File.Delete(_dbName);
            }
        }
    }

}
