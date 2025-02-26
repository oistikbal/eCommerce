using Bogus;
using eCommerce.UserService.Protos.V1;
using Grpc.AspNetCore.ClientFactory;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Service.V1
{
    public class AuthServiceTests : IClassFixture<UserServiceFixture>
    {
        private readonly Services.V1.AuthService _service;
        private readonly Faker<RegisterUserRequest> _registerUserFaker;

        public AuthServiceTests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _service = scope.ServiceProvider.GetRequiredService<Services.V1.AuthService>();

            _registerUserFaker = new Faker<RegisterUserRequest>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password(length: 20, memorable: false, prefix: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$"));
        }

        [Fact]
        public async Task RegisterUser_ValidRequest_ReturnsSucces()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task LoginUser_ValidRequest_ReturnsSucces()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };

            var loginResponse = await _service.Login(loginRequest, null);

            Assert.NotNull(response);
            Assert.True(response.Success);
        }
    }
}
