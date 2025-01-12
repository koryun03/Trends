using Microsoft.AspNetCore.Mvc;
using WebApplication121.ServiceInterfaces;

public class TrendsController : ControllerBase
{
    private readonly ITrendService _trendService;
    public TrendsController(ITrendService trendsService)
    {
        _trendService = trendsService;
    }

    [HttpGet("Trends")]
    public async Task<IActionResult> GetData(
        [FromQuery] string geo = "US",
        [FromQuery] int hours = 24,
        [FromQuery] int category = 0)
    {
        if (geo == "US" && category == 0)
        {
            return Ok(await _trendService.GetData(hours, category));
        }
        else
        {
            return Ok(await _trendService.GenerateTrendData(geo, hours, category));
        }
    }

}