using System.Text;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebApplication121.Common;
using WebApplication121.Models;
using WebApplication121.ServiceInterfaces;

namespace WebApplication121.Services
{
    public class TrendService : ITrendService
    {
        //private readonly IWebDriver _driver;
        private static IWebDriver _driver;
        private static HttpClient _client;
        public TrendService(IWebDriver driver, HttpClient client)
        {
            _driver = driver;
            _client = client;
        }

        public async Task<List<TrendRow>> GenerateTrendData(string geo, int hours, int category)
        {
            var builder = new StringBuilder($"https://trends.google.com/trending?geo={geo}&hours={hours}&sort=search-volume");
            if (category != 0)
            {
                builder.Append($"&category={category}");
            }
            var realUrl = builder.ToString();

            _driver.Navigate().GoToUrl(realUrl);

            var wait = new WebDriverWait(new SystemClock(), _driver, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(500));
            wait.Until(driver => driver.FindElements(By.CssSelector("tbody tr")).Count > 1);

            var trendData = new List<TrendRow>();

            var rows = _driver.FindElements(By.CssSelector("tbody tr"));
            await Task.Delay(300);
            for (int i = 1; i < 6; i++)
            {
                var row = rows[i];
                var trendRow = new TrendRow { RowNumber = i };

                try
                {
                    string name = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.jvkLtd"))
                                      .FindElement(By.CssSelector("div.mZ3RIc")).Text;
                    //   trendRow.Name = geo == "US" || geo == "GB" ? name : await TranslateToEnglishAsync(name);
                    trendRow.Name = (geo[0] == 'U' && geo[1] == 'S') || (geo[0] == 'G' && geo[1] == 'B') ? name
                        : await TranslateToEnglishAsync(name);

                    var targetCell = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.oQ5Nq.wFXHce"));
                    var svgElement = targetCell.FindElement(By.CssSelector("svg"));
                    trendRow.SvgHtml = svgElement.GetAttribute("outerHTML");
                    trendRow.BaseUrl = realUrl;
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Error extracting row data");
                }

                trendData.Add(trendRow);
            }
            return trendData;
        }

        public async Task<List<TrendRow>> GetData(int hours, int category)
        {
            if (hours == 24)
            {
                return TrendContext.Instance.Get("trend24");
            }
            if (hours == 48)
            {
                return TrendContext.Instance.Get("trend48");
            }
            if (hours == 168)
            {
                return TrendContext.Instance.Get("trend7");
            }
            else
            {
                return TrendContext.Instance.Get("trend4");
            }
        }


        private static async Task<string> TranslateToEnglishAsync(string text)
        {
            const string ApiBaseUrl = "http://lingva.ml/api/v1";
            string encodedText = Uri.EscapeDataString(text);
            string requestUrl = $"{ApiBaseUrl}/auto/en/{encodedText}";

            try
            {
                var response = await _client.GetAsync(requestUrl).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonDocument = JsonDocument.Parse(responseBody);
                return jsonDocument.RootElement.GetProperty("translation").GetString();
            }
            catch
            {
                return text;
            }
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
}
