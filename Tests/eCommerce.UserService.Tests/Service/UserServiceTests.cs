using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Service
{
    public class UserServiceTests : IClassFixture<UserServiceFixture>
    {
        private readonly UserService.Services.UserService _service;

        public UserServiceTests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _service = scope.ServiceProvider.GetRequiredService<UserService.Services.UserService>();
        }

        [Fact]
        public async Task RegisterUser_ValidRequest_ReturnsUserResponse()
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
            Assert.Equal(request.Username, response.Username);
            Assert.Equal(request.Email, response.Email);
        }
    }
}
