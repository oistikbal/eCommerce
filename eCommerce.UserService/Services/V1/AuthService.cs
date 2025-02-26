using System.Security.Claims;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.UserService.Services.V1
{
    public class AuthService : Protos.V1.AuthService.AuthServiceBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHelper _jwtHelper;

        public AuthService(UserManager<User> userManager, JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        public override async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            var response = new RegisterUserResponse
            {
                Success = result.Succeeded
            };

            response.Errors.AddRange(result.Errors.Select(e => e.Description));

            return response;
        }


        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
            }

            var token = _jwtHelper.GenerateToken(user);
            return new LoginResponse { Token = token };
        }

        [Authorize]
        public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            var userId = context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Errors = { "Unauthorized: Invalid or missing token." }
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Errors = { "User not found." }
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var response = new ChangePasswordResponse
                {
                    Success = false
                };

                response.Errors.AddRange(result.Errors.Select(e => e.Description));
            }

            return new ChangePasswordResponse { Success = true };
        }


    }
}
