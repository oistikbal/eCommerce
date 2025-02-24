using eCommerce.UserService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.UserService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

    }


}
