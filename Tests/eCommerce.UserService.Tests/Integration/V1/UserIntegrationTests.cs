using eCommerce.UserService.Data;
using eCommerce.UserService.Protos.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Integration.V1
{
    public class UserIntegrationV1Tests : IClassFixture<UserServiceFixture>
    {
        private readonly GrpcChannel _channel;

        public UserIntegrationV1Tests(UserServiceFixture factory)
        {
            var client = factory.CreateClient();
            _channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            var scope = factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        [Fact]
        public async Task CreateUser_ShouldSucces()
        {
            var client = new Protos.V1.AuthService.AuthServiceClient(_channel);

            var user = new RegisterUserRequest
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123*"
            };

            var response = await client.RegisterUserAsync(user);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }

    }
}
