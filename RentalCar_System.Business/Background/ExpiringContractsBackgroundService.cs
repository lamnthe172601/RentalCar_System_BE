using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentalCar_System.Business.RentalCarService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalCar_System.Business.Background
{
    public class ExpiringContractsBackgroundService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public ExpiringContractsBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(NotifyExpiringContracts, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async void NotifyExpiringContracts(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var rentalContractService = scope.ServiceProvider.GetRequiredService<IRentalContractService>();
                await rentalContractService.NotifyExpiringContractsAsync();
            }

            // Change the timer to not repeat
            _timer?.Change(Timeout.Infinite, 0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}