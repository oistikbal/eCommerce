using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.UserService.Tests.Model
{
    public class UserModelTests : IClassFixture<UserServiceFixture>
    {
        private ApplicationDbContext _dbContext;

        public UserModelTests(UserServiceFixture factory)
        {
            var scope = factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            var userRepository = new UserRepository(_dbContext);

            var user = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
            };

            var result = await userRepository.CreateUserAsync(user);

            var savedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user.UserName, result.UserName);
            Assert.Equal(user.Email, savedUser.Email);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser()
        {
            var userRepository = new UserRepository(_dbContext);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "testuser@example.com",
            };
            await userRepository.CreateUserAsync(user);

            var result = await userRepository.GetUserByUsernameAsync("testuser");

            Assert.NotNull(result);
            Assert.Equal(user.UserName, result.UserName);
        }
    }
}
