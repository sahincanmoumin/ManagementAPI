using DataAccessLayer.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            _logger.LogInformation("Animal LifeCycle Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var animalRepository = scope.ServiceProvider.GetRequiredService<IAnimalRepository>();

                        var expiredAnimals = await animalRepository.GetQueryable()
                            .Where(a => EF.Functions.DateDiffDay(a.PurchaseDate, DateTime.Now) >= a.LifeSpanDays)
                            .ToListAsync(stoppingToken);

                        foreach (var animal in expiredAnimals)
                        {
                            await animalRepository.DeleteAsync(animal);
                            _logger.LogInformation($"Animal {animal.Name} (ID: {animal.Id}) has reached its lifespan and was removed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing animal lifecycles.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("Animal LifeCycle Service is stopping.");
        }
    }
}