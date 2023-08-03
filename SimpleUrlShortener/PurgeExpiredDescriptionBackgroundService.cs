using System;
using SimpleUrlShortener.Services;

namespace SimpleUrlShortener
{
	public class PurgeExpiredDescriptionBackgroundService : BackgroundService
    {
        private readonly ILogger<PurgeExpiredDescriptionBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private const int oneHourDelay = 1 * 1000 * 3600; //3600sec;

        public PurgeExpiredDescriptionBackgroundService(IServiceProvider serviceProvider,
            ILogger<PurgeExpiredDescriptionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(oneHourDelay, stoppingToken);
                _logger.LogInformation("Suppression des urls expirée en cours.");
                using var scope = _serviceProvider.CreateScope();
                {
                    var urlShortenerService = scope.ServiceProvider.GetRequiredService<IUrlShortenerService>();
                    await urlShortenerService.PurgeExpiredDescription();
                }
            }
        }
    }
}

