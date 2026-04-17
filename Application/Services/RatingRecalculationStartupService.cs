using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RatingRecalculationStartupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RatingRecalculationStartupService> _logger;

    public RatingRecalculationStartupService(IServiceProvider services,
        ILogger<RatingRecalculationStartupService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

        using var scope = _services.CreateScope();
        var ratingService = scope.ServiceProvider.GetRequiredService<IRatingService>();

        _logger.LogInformation("Recalculating all book ratings on startup...");
        await ratingService.RecalculateAllAsync();
        _logger.LogInformation("Book ratings recalculated.");
    }
}
