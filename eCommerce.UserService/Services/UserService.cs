using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using eCommerce.UserService.Protos;
using Grpc.Core;

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

        public override async Task<UserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email
            };

            try
            {
                var createdUser = await _userRepository.CreateUserAsync(user);
                var response = new UserResponse
                {
                    Id = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

        }
    }
}
