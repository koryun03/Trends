using Microsoft.AspNetCore.Mvc;
using WebApplication121.ServiceInterfaces;

public class TrendsController : ControllerBase
{
    private static ITrendsService _trendsService;
    public TrendsController(ITrendsService trendsService)
    {
        _trendsService = trendsService;
    }

    [HttpGet("Trends")]
    public async Task<IActionResult> GetData(
        [FromQuery] string geo = "US",
        [FromQuery] int hours = 24,
        [FromQuery] int category = 0)
    {
        var data = await _trendsService.GenerateTrendsData(geo, hours, category);
        return Ok(data);
    }

    //private string GenerateLink(string name, string geo, int hour)
    //{
    //    string encodedName = WebUtility.UrlEncode(name);

    //    return hour switch
    //    {
    //        4 => $"https://trends.google.com/trends/explore?q={encodedName}&date=now%204-H&geo={geo}&hl=en-US",
    //        24 => $"https://trends.google.com/trends/explore?q={encodedName}&date=now%201-d&geo={geo}&hl=en-US",
    //        48 => $"https://trends.google.com/trends/explore?q={encodedName}&date=now%207-d&geo={geo}&hl=en-US",
    //        168 => $"https://trends.google.com/trends/explore?q={encodedName}&date=now%207-d&geo={geo}&hl=en-US",
    //        _ => ""
    //    };
    //}



}
