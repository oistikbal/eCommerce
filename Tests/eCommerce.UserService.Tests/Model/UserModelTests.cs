using eCommerce.UserService.Data;
using eCommerce.UserService.Data.Models;
using eCommerce.UserService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.UserService.Tests.Model
{
    public class UserModelTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var userRepository = new UserRepository(context);

            var user = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };

            var result = await userRepository.CreateUserAsync(user);

            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user.Username, savedUser.Username);
            Assert.Equal(user.Email, savedUser.Email);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser()
        {
            var options = CreateNewContextOptions();
            await using var context = new ApplicationDbContext(options);
            var userRepository = new UserRepository(context);

            var user = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };
            await userRepository.CreateUserAsync(user);

            var result = await userRepository.GetUserByUsernameAsync("testuser");

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
        }
    }
}
