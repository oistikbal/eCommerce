using Bogus;
using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using eCommerce.UserService.Protos.V1;

namespace eCommerce.UserService.Tests.Model
{
    public class UserModelTests : IClassFixture<UserServiceFactory>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly Faker<User> _fakeUser;

        public UserModelTests(UserServiceFactory fixture)
        {
            var scope = fixture.Services.CreateScope();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _fakeUser = new Faker<User>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());
        }

        [Fact]
        public async Task CreateUser_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            var result = await _userManager.CreateAsync(user, "StrongP@ssword123");
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            await _userManager.CreateAsync(user, "StrongP@ssword123");
            var result = await _userManager.FindByNameAsync(user.UserName);
            Assert.NotNull(result);
            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            await _userManager.CreateAsync(user, "StrongP@ssword123");
            var result = await _userManager.FindByEmailAsync(user.Email);
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task DeleteUser_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            await _userManager.CreateAsync(user, "StrongP@ssword123");
            var result = await _userManager.DeleteAsync(user);
            Assert.True(result.Succeeded);
            var deletedUser = await _userManager.FindByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task CheckPassword_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            string password = "SecureP@ss123";
            await _userManager.CreateAsync(user, password);
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            Assert.True(isPasswordCorrect);
        }

        [Fact]
        public async Task ChangePassword_WhenRequestIsValid_ShouldSuccess()
        {
            var user = _fakeUser.Generate();
            string oldPassword = "OldP@ss123";
            string newPassword = "NewP@ss456";
            await _userManager.CreateAsync(user, oldPassword);
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            Assert.True(result.Succeeded);
            Assert.False(await _userManager.CheckPasswordAsync(user, oldPassword));
            Assert.True(await _userManager.CheckPasswordAsync(user, newPassword));
        }

        [Fact]
        public async Task CreateUser_WhenDuplicateUsername_ShouldFail()
        {
            var user1 = _fakeUser.Generate();
            var user2 = new User
            {
                UserName = user1.UserName,
                Email = _fakeUser.Generate().Email
            };

            await _userManager.CreateAsync(user1, "StrongP@ssword123");
            var result = await _userManager.CreateAsync(user2, "StrongP@ssword123");

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Code == "DuplicateUserName");
        }

        [Fact]
        public async Task CreateUser_WhenDuplicateEmail_ShouldFail()
        {
            var user1 = _fakeUser.Generate();
            var user2 = new User
            {
                UserName = _fakeUser.Generate().UserName,
                Email = user1.Email
            };

            await _userManager.CreateAsync(user1, "StrongP@ssword123");
            var result = await _userManager.CreateAsync(user2, "StrongP@ssword123");

            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
        }
    }
}
