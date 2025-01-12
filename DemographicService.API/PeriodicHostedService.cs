using Demographic.Infrastructure.Servieces.Contracts;
using Polly;
using Polly.Retry;

public class PeriodicHostedService(ILogger<PeriodicHostedService> logger, 
    IHttpClientFactory httpClientFactory,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly PeriodicTimer timer = new(TimeSpan.FromSeconds(15));

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            {
                using var scope = serviceScopeFactory.CreateScope();
                var geoService = scope.ServiceProvider.GetRequiredService<IGeoService>();
                var populationService = scope.ServiceProvider.GetRequiredService<IPopulationService>();
                var statePopulation = await GetPopulationDataAsync(geoService, cancellationToken);
                //save in database - upsert
                await populationService.SaveStatePopulationsAsync(statePopulation, cancellationToken);

            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in hosted service happend");
        }
        
    }

    private async Task<Dictionary<string, int>> GetPopulationDataAsync(IGeoService geoService, CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();
       
        return await GetRetryPolicy().ExecuteAsync(async () =>
        {
            return await geoService.GetPopulationsAsync(client);
        });
    }

    private AsyncRetryPolicy GetRetryPolicy() =>
         Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                });
}
