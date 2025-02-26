using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.UserService.Services.V1
{
    public class UserService : Protos.V1.UserService.UserServiceBase
    {
        private readonly ILogger<AuthService> _logger;

        public UserService(UserManager<User> userManager, ILogger<AuthService> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
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
