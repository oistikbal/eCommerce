using eCommerce.UserService.Protos.V1;
using Grpc.Net.Client;

namespace eCommerce.UserService.Tests.Integration.V1
{
    public class UserIntegrationTests : IClassFixture<UserServiceFixture>
    {
        private readonly GrpcChannel _channel;

        public UserIntegrationTests(UserServiceFixture factory)
        {
            var client = factory.CreateClient();
            _channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }

        [Fact]
        public async Task CheckHealth_WhenRequestIsValid_ShouldSuccess()
        {
            var client = new Protos.V1.UserService.UserServiceClient(_channel);


            var response = await client.CheckHealthAsync(new HealthCheckRequest());

            Assert.NotNull(response);
            Assert.Equal("v1.0", response.Version);
        }

    }
}
