using eCommerce.ProductService.Data;
using eCommerce.ProductService.Data.Models;
using eCommerce.ProductService.Protos.V1;
using Grpc.Core;

namespace eCommerce.ProductService.Services.V1
{
    public class ProductService : Protos.V1.ProductService.ProductServiceBase
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = (decimal)request.Price,
                Stock = request.Stock,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new CreateProductResponse
            {
                Success = true,
            };
        }
    }
}
