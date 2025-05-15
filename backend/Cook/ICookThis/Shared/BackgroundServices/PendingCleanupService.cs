using System;
using System.Threading;
using System.Threading.Tasks;
using ICookThis.Modules.Auth.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ICookThis.Shared.BackgroundServices
{
    public class PendingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<PendingCleanupService> _logger;

        public PendingCleanupService(
            IServiceProvider provider,
            ILogger<PendingCleanupService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PendingCleanupService started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();

                    await auth.CleanupPendingAsync();
                    _logger.LogInformation("CleanupPendingAsync executed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during CleanupPendingAsync");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
