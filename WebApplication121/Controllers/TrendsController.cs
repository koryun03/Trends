using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebApplication121.Models;

public class TrendsController : ControllerBase/*, IDisposable*/
{
    private static IWebDriver _driver;

    public TrendsController()
    {
        if (_driver == null)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            _driver = new ChromeDriver(options);
        }
    }

    [HttpGet("{pageNumber}")]
    public async Task<IActionResult> GetData(
        int pageNumber = 0,
        [FromQuery] string geo = "US",
        [FromQuery] int hours = 24,
        [FromQuery] int category = 0)
    {
        var builder = new StringBuilder($"https://trends.google.com/trending?geo={geo}&hours={hours}");
        if (category != 0)
        {
            builder.Append($"&category={category}");
        }
        var realUrl = builder.ToString();

        _driver.Navigate().GoToUrl(realUrl);

        var wait = new WebDriverWait(new SystemClock(), _driver, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(500));
        wait.Until(driver => driver.FindElements(By.CssSelector("tbody tr")).Count > 1);

        var trendData = new List<TrendRow>();

        if (pageNumber > 0)
        {
            try
            {
                var button = _driver.FindElement(By.CssSelector("button[aria-label='Go to next page']"));
                for (int i = 0; i < pageNumber; i++)
                {
                    button.Click();

                    wait.Until(driver => driver.FindElements(By.CssSelector("tbody tr")).Count > 1);
                }
            }
            catch (NoSuchElementException)
            {
                return BadRequest("Button was not found.");
            }
        }

        var rows = _driver.FindElements(By.CssSelector("tbody tr"));

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            var trendRow = new TrendRow { RowNumber = i };

            try
            {
                string name = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.jvkLtd"))
                                  .FindElement(By.CssSelector("div.mZ3RIc")).Text;
                trendRow.Name = geo == "US" ? name : await TranslateToEnglishAsync(name);

                trendRow.SearchVolume = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.dQOTjf div.p6GDQc div.lqv0Cb")).Text;
                trendRow.Percent = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.dQOTjf div.wqrjjc div.TXt85b")).Text;
                trendRow.StartedAt = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.WirRge div.vdw3Ld")).Text;
                trendRow.Status = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.WirRge div.UQMqQd")).Text;

                var targetCell = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.oQ5Nq.wFXHce"));
                var svgElement = targetCell.FindElement(By.CssSelector("svg"));
                trendRow.SvgHtml = svgElement.GetAttribute("outerHTML");

                var cell = row.FindElement(By.XPath("//div[@class='enOdEe-wZVHld-gruSEe-j4LONd' and contains(@aria-label, 'of')]"));
                trendRow.RowsAndPagesData = cell.GetAttribute("aria-label");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Error extracting row data");
            }

            trendData.Add(trendRow);
        }

        return Ok(trendData);
    }
    //~TrendsController()
    //{
    //    _driver?.Quit();
    //}
    private static async Task<string> TranslateToEnglishAsync(string text)
    {
        const string ApiBaseUrl = "http://lingva.ml/api/v1";
        using var client = new HttpClient();
        string encodedText = Uri.EscapeDataString(text);
        string requestUrl = $"{ApiBaseUrl}/auto/en/{encodedText}";

        try
        {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseBody);
            return jsonDocument.RootElement.GetProperty("translation").GetString();
        }
        catch
        {
            return text;
        }
    }

    //public void Dispose()
    //{
    //    _driver?.Quit();
    //}
}
