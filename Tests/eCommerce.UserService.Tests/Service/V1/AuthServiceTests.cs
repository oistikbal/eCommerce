using Bogus;
using eCommerce.UserService.Protos.V1;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Service.V1
{
    public class AuthServiceTests : IClassFixture<UserServiceFixture>
    {
        private readonly Services.V1.AuthService _service;
        private readonly Faker<RegisterUserRequest> _registerUserFaker;

        public AuthServiceTests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _service = scope.ServiceProvider.GetRequiredService<Services.V1.AuthService>();

            _registerUserFaker = new Faker<RegisterUserRequest>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password(length: 20, memorable: false, prefix: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$"));
        }

        [Fact]
        public async Task RegisterUser_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task RegisterUser_WhenEmailIsValid_ShouldFail()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task RegisterUser_WhenEmailIsDuplicate_ShouldFail()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            var secondUser = _registerUserFaker.Generate();

            request = new RegisterUserRequest
            {
                Username = secondUser.Username,
                Password = secondUser.Password,
                Email = user.Email
            };

            var registerResponse = await _service.RegisterUser(request, null);

            Assert.True(response.Success);
            Assert.NotEmpty(registerResponse.Errors);
        }

        [Fact]
        public async Task RegisterUser_WhenEmailIsInvalid_ShouldFail()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = "dasdsa"
            };

            var response = await _service.RegisterUser(request, null);

            Assert.NotNull(response);
            Assert.NotEmpty(response.Errors);
        }

        [Fact]
        public async Task LoginUser_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };

            var loginResponse = await _service.Login(loginRequest, null);

            Assert.True(response.Success);
            Assert.True(loginResponse.Success);
        }

        [Fact]
        public async Task LoginUser_WhenRequestIsInValid_ShouldFail()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(request, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = "dasdasdas"
            };

            var loginResponse = await _service.Login(loginRequest, null);

            Assert.True(response.Success);
            Assert.False(loginResponse.Success);
        }

        [Fact]
        public async Task ChangePassword_WhenPasswordIsCorrect_ShouldSuccess()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var registerResponse = await _service.RegisterUser(user, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };


            var loginResponse = await _service.Login(loginRequest, null);

            var header = new Metadata
            {
                { "Authorization", $"Bearer {loginResponse.Token}" }
            };

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = request.Password,
                NewPassword = _registerUserFaker.Generate().Password
            };

            var context = TestServerCallContext.Create(
                method: "ChangePassword",
                host: "localhost",
                deadline: DateTime.UtcNow.AddMinutes(30),
                requestHeaders: header,
                cancellationToken: CancellationToken.None,
                peer: "ipv4:127.0.0.1:5000",
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: (metadata) => Task.CompletedTask,
                writeOptionsGetter: () => null,
                writeOptionsSetter: (options) => { }
            );

            var changePasswordResponse = await _service.ChangePassword(changePasswordRequest, context);

            Assert.True(registerResponse.Success);
            Assert.True(loginResponse.Success);
            Assert.Empty(changePasswordResponse.Errors);
        }

        [Fact]
        public async Task ChangePassword_WhenPasswordIsIncorrect_ShouldFail()
        {
            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var registerResponse = await _service.RegisterUser(user, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };


            var loginResponse = await _service.Login(loginRequest, null);

            var header = new Metadata
            {
                { "Authorization", $"Bearer {loginResponse.Token}" }
            };

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = request.Password,
                NewPassword = "123"
            };

            var context = TestServerCallContext.Create(
                method: "ChangePassword",
                host: "localhost",
                deadline: DateTime.UtcNow.AddMinutes(30),
                requestHeaders: header,
                cancellationToken: CancellationToken.None,
                peer: "ipv4:127.0.0.1:5000",
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: (metadata) => Task.CompletedTask,
                writeOptionsGetter: () => null,
                writeOptionsSetter: (options) => { }
            );

            var changePasswordResponse = await _service.ChangePassword(changePasswordRequest, context);


            Assert.True(registerResponse.Success);
            Assert.True(loginResponse.Success);
            Assert.NotEmpty(changePasswordResponse.Errors);
        }

        [Fact]
        public async Task ChangeEmail_WhenRequestIsCorrect_ShouldSucces()
        {

            var user = _registerUserFaker.Generate();

            var request = new RegisterUserRequest
            {
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var response = await _service.RegisterUser(user, null);

            var loginRequest = new LoginRequest()
            {
                Email = request.Email,
                Password = request.Password
            };


            var loginResponse = await _service.Login(loginRequest, null);

            var header = new Metadata
            {
                { "Authorization", $"Bearer {loginResponse.Token}" }
            };

            var changeEmailRequest = new ChangeEmailRequest
            {
                Password = request.Password,
                NewEmail = _registerUserFaker.Generate().Email
            };

            var context = TestServerCallContext.Create(
                method: "ChangeEmail",
                host: "localhost",
                deadline: DateTime.UtcNow.AddMinutes(30),
                requestHeaders: header,
                cancellationToken: CancellationToken.None,
                peer: "ipv4:127.0.0.1:5000",
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: (metadata) => Task.CompletedTask,
                writeOptionsGetter: () => null,
                writeOptionsSetter: (options) => { }
            );

            var changeEmailResponse = await _service.ChangeEmail(changeEmailRequest, context);

            Assert.Empty(changeEmailResponse.Errors);
        }
    }
}
