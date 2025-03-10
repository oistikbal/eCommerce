using System.Text.Json;
using eCommerce.ProductService.Data;
using eCommerce.ProductService.Data.Models;
using eCommerce.ProductService.Protos.V1;
using eCommerce.Shared;
using Grpc.Core;

namespace eCommerce.ProductService.Services.V1
{
    public class ProductService : Protos.V1.ProductService.ProductServiceBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RabbitMQProducer _rabbitMQProducer;

        public ProductService(ApplicationDbContext context, RabbitMQProducer rabbitMQProducer)
        {
            _context = context;
            _rabbitMQProducer = rabbitMQProducer;
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

            var productCreatedMessage = new
            {
                EventType = "ProductCreated",
                Product = new
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Serialize message to JSON
            string messageJson = JsonSerializer.Serialize(productCreatedMessage);

            // Send message to RabbitMQ
            await _rabbitMQProducer.SendMessageAsync(
                exchange: "product-events",
                routingKey: "product.created",
                message: messageJson);

            return new CreateProductResponse
            {
                Success = true,
            };
        }
    }
}
