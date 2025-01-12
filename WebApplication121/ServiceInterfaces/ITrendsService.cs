using WebApplication121.Models;

namespace WebApplication121.ServiceInterfaces
{
    public interface ITrendsService
    {
        Task<List<TrendRow>> GenerateTrendsData(string geo = "US", int hours = 24, int category = 0);
        Task<List<TrendRow>> GetTrendsDataFromDb(string geo = "US", int hours = 24, int category = 0);
    }
}
