using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eCommerce.Shared;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace eCommerce.UserService.Services.V1
{
    public class AuthService : Protos.V1.AuthService.AuthServiceBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHelper _jwtHelper;
        private readonly IStringLocalizer<AuthService> _localizer;

        public AuthService(UserManager<User> userManager, JwtHelper jwtHelper, IStringLocalizer<AuthService> localizer)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
            _localizer = localizer;
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
                return new LoginResponse { Success = false, Token = string.Empty };
            }

            var token = _jwtHelper.GenerateToken(user);
            return new LoginResponse { Success = true, Token = token };
        }

        public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            var token = context.RequestHeaders.GetValue("Authorization")?.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Errors = { _localizer["InvalidToken"] }
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Errors = { _localizer["UserNotFound"] }
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

                return response;
            }

            return new ChangePasswordResponse { Success = true };
        }

        public override async Task<ChangeEmailResponse> ChangeEmail(ChangeEmailRequest request, ServerCallContext context)
        {
            var token = context.RequestHeaders.GetValue("Authorization")?.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new ChangeEmailResponse { Success = false, Errors = { _localizer["InvalidToken"] } };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ChangeEmailResponse { Success = false, Errors = { _localizer["UserNotFound"] } };
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordCheck)
            {
                return new ChangeEmailResponse { Success = false, Errors = { _localizer["InvalidPassword"] } };
            }

            var result = await _userManager.SetEmailAsync(user, request.NewEmail);
            if (!result.Succeeded)
            {
                var response = new ChangeEmailResponse
                {
                    Success = false,
                };

                response.Errors.AddRange(result.Errors.Select(e => e.Description));

                return response;
            }

            return new ChangeEmailResponse { Success = true };
        }

        public override async Task<RoleResponse> CheckUserRole(RoleRequest request, ServerCallContext context)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                return new RoleResponse { HasRole = false };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new RoleResponse { HasRole = roles.Contains(request.Role) };
        }
    }
}
