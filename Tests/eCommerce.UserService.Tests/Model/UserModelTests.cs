using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Model
{
    public class UserModelTests : IClassFixture<UserServiceFixture>
    {
        private ApplicationDbContext _dbContext;
        private UserManager<User> _userManager;

        public UserModelTests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            var user = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
            };

            await _userManager.CreateAsync(user);

            var savedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            Assert.NotNull(savedUser);
            Assert.Equal(user.UserName, user.UserName);
            Assert.Equal(user.Email, savedUser.Email);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "testuser@example.com",
            };
            await _userManager.CreateAsync(user);

            var result = await _userManager.FindByNameAsync("testuser");

            Assert.NotNull(result);
            Assert.Equal(user.UserName, result.UserName);
        }
    }
}
