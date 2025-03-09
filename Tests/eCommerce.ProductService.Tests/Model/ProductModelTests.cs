using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using eCommerce.ProductService.Data;
using eCommerce.ProductService.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

namespace eCommerce.ProductService.Tests.Model
{
    public class ProductModelTests : IClassFixture<ProductServiceFactory>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Faker<Product> _fakeProduct;

        public ProductModelTests(ProductServiceFactory factory)
        {
            var scope = factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _fakeProduct = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Price, f => f.Random.Decimal(5, 500))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 1000))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
                .RuleFor(p => p.UpdatedAt, f => f.Date.Recent(30).ToUniversalTime());
        }

        [Fact]
        public async Task CreateProduct_WhenRequestIsValid_ShouldSuccess()
        {
            var product = _fakeProduct.Generate();

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            var savedProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.Name, savedProduct?.Name);
            Assert.Equal(product.Price, savedProduct?.Price);
        }
    }
}
