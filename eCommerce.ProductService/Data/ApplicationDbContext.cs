using eCommerce.ProductService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.ProductService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
