using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using eCommerce.UserService.Services.V1;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Service.V1
{
    public class UserServiceV1Tests : IClassFixture<UserServiceFixture>
    {
        private readonly UserServiceV1 _service;

        public UserServiceV1Tests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _service = scope.ServiceProvider.GetRequiredService<UserServiceV1>();
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
