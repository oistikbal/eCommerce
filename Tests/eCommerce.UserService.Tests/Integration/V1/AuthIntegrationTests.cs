using Bogus;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;

namespace eCommerce.UserService.Tests.Integration.V1
{
    public class AuthIntegrationTests : IClassFixture<UserServiceFactory>
    {
        private readonly GrpcChannel _channel;
        private readonly Faker<RegisterUserRequest> _registerUserFaker;

        public AuthIntegrationTests(UserServiceFactory factory)
        {
            var client = factory.CreateClient();
            _channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            _registerUserFaker = new Faker<RegisterUserRequest>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password(length: 20, memorable: false, prefix: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$"));
        }

        [Fact]
        public async Task CreateUser_WhenRequestIsValid_ShouldSucces()
        {
            var client = new Protos.V1.AuthService.AuthServiceClient(_channel);

            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await client.RegisterUserAsync(user);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task ChangePassword_WhenRequestIsValid_ShouldSucces()
        {
            var client = new Protos.V1.AuthService.AuthServiceClient(_channel);

            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await client.RegisterUserAsync(user);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };


            var loginResponse = await client.LoginAsync(loginRequest);

            var header = new Metadata
            {
                { "Authorization", $"Bearer {loginResponse.Token}" }
            };

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = request.Password,
                NewPassword = _registerUserFaker.Generate().Password
            };

            var changePasswordResponse = await client.ChangePasswordAsync(changePasswordRequest, headers: header);

            Assert.Empty(changePasswordResponse.Errors);
        }

        [Fact]
        public async Task ChangeEmail_WhenRequestIsValid_ShouldSucces()
        {
            var client = new Protos.V1.AuthService.AuthServiceClient(_channel);

            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await client.RegisterUserAsync(user);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };


            var loginResponse = await client.LoginAsync(loginRequest);

            var header = new Metadata
            {
                { "Authorization", $"Bearer {loginResponse.Token}" }
            };

            var changeEmailRequest = new ChangeEmailRequest
            {
                Password = request.Password,
                NewEmail = _registerUserFaker.Generate().Email
            };

            var changeEmailResponse = await client.ChangeEmailAsync(changeEmailRequest, headers: header);

            Assert.Empty(changeEmailResponse.Errors);
        }

    }
}
