using WebApplication121.Models;

namespace WebApplication121.ServiceInterfaces
{
    public interface ITrendService
    {
        Task<List<TrendRow>> GenerateTrendData(string geo, int hours, int category);
        Task<List<TrendRow>> GetData(int hours, int category);
    }
}
