using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using eCommerce.UserService.Protos.V1;
using Grpc.Core.Testing;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.E2E.V1
{
    public class AuthServiceTests : IClassFixture<UserServiceFactory>
    {
        private readonly Services.V1.AuthService _service;
        private readonly Faker<RegisterUserRequest> _registerUserFaker;

        public AuthServiceTests(UserServiceFactory factory)
        {
            var client = factory.CreateClient();
            var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            _service = factory.Services.CreateScope().ServiceProvider.GetRequiredService<Services.V1.AuthService>();

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

    }
}
