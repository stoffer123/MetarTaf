using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DecoderTesting
{
    public class InfoStationUpdater : BackgroundService
    {
        private readonly IInfoStation _infoStation;

        public InfoStationUpdater(IInfoStation infoStation)
        {
            _infoStation = infoStation;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("Fetching reports...");
                    await _infoStation.loadReports();
                    Console.WriteLine("Reports loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in InfoStationUpdater: {ex.Message}");
                }

                // Wait for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}