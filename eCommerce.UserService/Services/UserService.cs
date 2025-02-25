using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.UserService.Services
{
    public class UserService : Protos.UserService.UserServiceBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task<UserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email
            };


            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new RpcException(new Status(StatusCode.Internal, "User creation failed"));
            }

            return new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }
    }
}
