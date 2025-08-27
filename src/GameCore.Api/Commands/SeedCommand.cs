using GameCore.Scripts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Commands;

public class SeedCommand : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SeedCommand> _logger;

    public SeedCommand(IServiceProvider serviceProvider, ILogger<SeedCommand> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();
            
            _logger.LogInformation("開始執行資料填充...");
            await seedData.SeedAllDataAsync();
            _logger.LogInformation("資料填充完成！");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "資料填充失敗");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}