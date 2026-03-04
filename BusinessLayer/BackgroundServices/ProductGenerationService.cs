using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Entity.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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

                        var animals = await animalRepository.GetQueryable().ToListAsync(stoppingToken);

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

                                await productRepository.AddAsync(product);

                                animal.LastProductionDate = DateTime.Now;

                                await animalRepository.UpdateAsync(animal);

                                _logger.LogInformation($"Product {product.Name} generated from {animal.Name} (ID: {animal.Id})");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ProductGenerationService");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private ProductType GetProductName(AnimalType animalType)
        {
            return animalType switch
            {
                AnimalType.Cow => ProductType.Milk,
                AnimalType.Chicken => ProductType.Egg,
                AnimalType.Sheep => ProductType.Wool,
                _ => ProductType.Milk
            };
        }

        private decimal GetProductPrice(AnimalType animalType)
        {
            return animalType switch
            {
                AnimalType.Cow => 10,
                AnimalType.Chicken => 2,
                AnimalType.Sheep => 15,
                _ => 5
            };
        }
    }
}