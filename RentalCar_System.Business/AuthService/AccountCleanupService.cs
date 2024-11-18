using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RentalCar_System.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalCar_System.Business.AuthService
{
    
    public class AccountCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountCleanupService> _logger;

        public AccountCleanupService(IServiceProvider serviceProvider, ILogger<AccountCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupUnverifiedAccounts();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Run once a day
            }
        }

        private async Task CleanupUnverifiedAccounts()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RentalCarDBContext>();

                var unverifiedUsers = dbContext.Users
                    .Where(u => !dbContext.Tokens.Any(t => t.UserId == u.UserId && t.IsUsed) && u.CreatedAt < DateTime.UtcNow.AddDays(-1))
                    .ToList();

                if (unverifiedUsers.Any())
                {
                    dbContext.Users.RemoveRange(unverifiedUsers);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Deleted {unverifiedUsers.Count} unverified user accounts.");
                }
            }
        }
    }

}
