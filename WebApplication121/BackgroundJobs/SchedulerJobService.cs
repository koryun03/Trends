using WebApplication121.Infrastructure;

namespace WebApplication121.BackgroundJobs
{
    public class SchedulerJobService : BackgroundService
    {
        private readonly ILogger<SchedulerJobService> _logger;
        private readonly MongoDbContext _context;

        public SchedulerJobService(ILogger<SchedulerJobService> logger, MongoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Update Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting data update...");

                    // Получаем новые данные (например, из API)
                    // var newData = await FetchDataFromApiAsync();

                    // Обновляем данные в MongoDB
                    //foreach (var item in newData)
                    //{
                    //    var filter = Builders<TrendRow>.Filter.Eq(x => x.Id, item.Id);
                    //    var options = new ReplaceOptions { IsUpsert = true }; // Вставить, если отсутствует
                    //    //await _collection.ReplaceOneAsync(filter, item, options, stoppingToken);
                    //  //  await _context.TrendsData.ReplaceOneAsync(filter, item, options, stoppingToken);
                    //}

                    _logger.LogInformation("Data update completed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating data.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        //private async Task<TrendRow[]> FetchDataFromApiAsync()
        //{
        //    // Пример: симулируем загрузку данных
        //    await Task.Delay(1000); // Задержка для имитации HTTP-запроса
        //    return new[]
        //    {
        //     new TrendRow { Id = 1, Name = "Data 1", UpdatedAt = DateTime.UtcNow },
        //     new TrendRow { Id = 2, Name = "Data 2", UpdatedAt = DateTime.UtcNow },
        // };
        //}
    }
}
