using Bogus;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using eCommerce.UserService.Services.V1;
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
                .RuleFor(u => u.Password, f => f.Internet.Password());
        }

        [Fact]
        public async Task RegisterUser_ValidRequest_ReturnsSucces()
        {
            var request = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "Password123*",
                Email = "test@example.com"
            };

            var createdUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Username,
                Email = request.Email
            };

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }
    }
}
