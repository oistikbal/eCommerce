using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace eCommerce.UserService.Services
{
    public class UserService : Protos.UserService.UserServiceBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public override Task<UserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Registering user: {Username}", request.Username);

            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Şifreyi burada düz bırakıyoruz, ancak hashlenmeli
                Email = request.Email
            };

            var createdUser = _userRepository.CreateUserAsync(user).Result;

            var response = new UserResponse
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email
            };

            return Task.FromResult(response);
        }
    }
}
