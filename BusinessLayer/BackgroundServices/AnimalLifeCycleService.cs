using DataAccessLayer.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.BackgroundServices
{
    public class AnimalLifeCycleService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnimalLifeCycleService> _logger;

        public AnimalLifeCycleService(IServiceProvider serviceProvider, ILogger<AnimalLifeCycleService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Animal LifeCycle Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var animalRepository = scope.ServiceProvider.GetRequiredService<IAnimalRepository>();
                        var animals = animalRepository.GetAll();

                        foreach (var animal in animals)
                        {
                            var daysSincePurchase = (DateTime.Now - animal.PurchaseDate).Days;

                            if (daysSincePurchase >= animal.LifeSpanDays)
                            {
                                animalRepository.Delete(animal);
                                _logger.LogInformation($"Animal {animal.Name} (ID: {animal.Id}) died after {daysSincePurchase} days");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in AnimalLifeCycleService");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}