using eCommerce.UserService.Data;
using eCommerce.UserService.Protos;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Service
{
    public class UserIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly GrpcChannel _channel;

        public UserIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var client = factory.CreateClient();
            _channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            using var scope = factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.EnsureCreated(); // Test için yeni DB oluştur
        }

        [Fact]
        public async Task CreateUser_ShouldReturnUser()
        {
            var client = new UserService.Protos.UserService.UserServiceClient(_channel);

            var user = new RegisterUserRequest
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };

            var response = await client.RegisterUserAsync(user);

            Assert.NotNull(response);
            Assert.Equal(user.Username, response.Username);
            Assert.Equal(user.Email, response.Email);
        }

    }
}
