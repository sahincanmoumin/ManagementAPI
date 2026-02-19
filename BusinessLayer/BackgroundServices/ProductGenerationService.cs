using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer.BackgroundServices
{
    public class ProductGenerationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProductGenerationService> _logger;

        public ProductGenerationService(IServiceProvider serviceProvider, ILogger<ProductGenerationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Product Generation Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var animalRepository = scope.ServiceProvider.GetRequiredService<IAnimalRepository>();
                        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

                        var animals = animalRepository.GetAll();

                        foreach (var animal in animals)
                        {
                            var hoursSinceLastProduction = (DateTime.Now - animal.LastProductionDate).TotalHours;

                            if (hoursSinceLastProduction >= animal.ProductionIntervalHours)
                            {
                                var product = new Product
                                {
                                    Name = GetProductName(animal.Type),
                                    Price = GetProductPrice(animal.Type),
                                    AnimalId = animal.Id,
                                    ProducedAt = DateTime.Now,
                                    IsSold = false
                                };

                                productRepository.Add(product);

                                animal.LastProductionDate = DateTime.Now;
                                animalRepository.Update(animal);

                                _logger.LogInformation($"Product {product.Name} generated from {animal.Name} (ID: {animal.Id})");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ProductGenerationService");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private string GetProductName(string animalType)
        {
            return animalType.ToLower() switch
            {
                "cow" => "Milk",
                "chicken" => "Egg",
                "sheep" => "Wool",
                _ => "Product"
            };
        }

        private decimal GetProductPrice(string animalType)
        {
            return animalType.ToLower() switch
            {
                "cow" => 10,
                "chicken" => 2,
                "sheep" => 15,
                _ => 5
            };
        }
    }
}