using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.UserService.Services.V1
{
    public class UserServiceV1 : Protos.V1.UserService.UserServiceBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserServiceV1> _logger;

        public UserServiceV1(UserManager<User> userManager, ILogger<UserServiceV1> logger, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
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

            await _applicationDbContext.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }

        public override Task<HealthCheckResponse> CheckHealth(HealthCheckRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Health check requested.");

            return Task.FromResult(new HealthCheckResponse
            {
                Status = "OK",
                Version = "v1.0"
            });
        }
    }
}
