using MongoDB.Driver;
using WebApplication121.Entities;

namespace WebApplication121.Infrastructure
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public MongoDbContext()
        {
            var mongoclient = new MongoClient("mongodb://localhost:27017");
            _database = mongoclient.GetDatabase("Trends");
        }
        public IMongoCollection<TrendsData> TrendsData => _database.GetCollection<TrendsData>("TrendsData");

    }
}
