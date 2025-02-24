using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace eCommerce.UserService.Service.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<UserService.Services.UserService>> _logger;
        private readonly UserService.Services.UserService _service;

        public UserServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<UserService.Services.UserService>>();
            _service = new UserService.Services.UserService(_userRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task RegisterUser_ValidRequest_ReturnsUserResponse()
        {
            var request = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com"
            };

            var createdUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            _userRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                           .ReturnsAsync(createdUser);

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.Equal(1, response.Id);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("test@example.com", response.Email);
        }

        [Fact]
        public async Task RegisterUser_RepositoryThrowsException_ThrowsRpcException()
        {
            var request = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com"
            };

            _userRepository.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                           .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await _service.RegisterUser(request, null));
            Assert.Contains("Database error", exception.Message);
        }
    }
}