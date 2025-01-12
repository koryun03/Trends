using WebApplication121.Common;
using WebApplication121.Models;
using WebApplication121.ServiceInterfaces;

namespace WebApplication121.Services
{
    public class SchedulerJobService : BackgroundService
    {
        private readonly ILogger<SchedulerJobService> _logger;
        private readonly ITrendService _trendService;
        public SchedulerJobService(ILogger<SchedulerJobService> logger, ITrendService trendsService)
        {
            _logger = logger;
            _trendService = trendsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Update Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting data update...");

                    TrendContext.Instance.Remove("trend4");
                    List<TrendRow> trends4h = await _trendService.GenerateTrendData("US", 4, 0);
                    TrendContext.Instance.AddRange("trend4", trends4h);

                    TrendContext.Instance.Remove("trend24");
                    List<TrendRow> trends24h = await _trendService.GenerateTrendData("US", 24, 0);
                    TrendContext.Instance.AddRange("trend24", trends24h);

                    TrendContext.Instance.Remove("trend7");
                    List<TrendRow> trends7h = await _trendService.GenerateTrendData("US", 24, 0);
                    TrendContext.Instance.AddRange("trend7", trends7h);

                    TrendContext.Instance.Remove("trend48");
                    List<TrendRow> trends48h = await _trendService.GenerateTrendData("US", 24, 0);
                    TrendContext.Instance.AddRange("trend48", trends48h);

                    _logger.LogInformation("Data update completed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating data.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
