namespace theforum.Whitelabel;

public class FaviconInitializationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public FaviconInitializationService(IServiceProvider serviceProvider) => 
        _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var faviconService = scope.ServiceProvider.GetRequiredService<FaviconService>();
        await faviconService.InitializeAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => 
        Task.CompletedTask;
}