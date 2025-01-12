using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebApplication121.Models;
using WebApplication121.ServiceInterfaces;

namespace WebApplication121.Services
{
    public class TrendsService : ITrendsService
    {
        private readonly IWebDriver _driver;
        private readonly ITranslateService _translateService;
        public TrendsService(IWebDriver driver, ITranslateService translateService)
        {
            _driver = driver;
            _translateService = translateService;
        }
        public async Task<List<TrendRow>> GenerateTrendsData(string geo = "US", int hours = 24, int category = 0)
        {
            var builder = new StringBuilder($"https://trends.google.com/trending?geo={geo}&hours={hours}&sort=search-volume");
            if (category != 0)
            {
                builder.Append($"&category={category}");
            }
            var realUrl = builder.ToString();

            _driver.Navigate().GoToUrl(realUrl);
            //IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            //js.ExecuteScript(@"
            //    var stylesheets = document.styleSheets;
            //    for (var i = stylesheets.length - 1; i >= 0; i--) {
            //        stylesheets[i].disabled = true;
            //    }
            //");


            var wait = new WebDriverWait(new SystemClock(), _driver, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(500));
            wait.Until(driver => driver.FindElements(By.CssSelector("tbody tr")).Count > 1);

            var trendData = new List<TrendRow>();

            var rows = _driver.FindElements(By.CssSelector("tbody tr"));
            await Task.Delay(300); ////??????????????
                                   //for (int i = a; i < rowsCount; i++)
            for (int i = 1; i < 6; i++)
            {
                var row = rows[i];
                var trendRow = new TrendRow { RowNumber = i };

                try
                {
                    string name = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.jvkLtd"))
                                      .FindElement(By.CssSelector("div.mZ3RIc")).Text;
                    trendRow.Name = geo == "US" || geo == "GB" ? name : await _translateService.TranslateToEnglishAsync(name);

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
    }
}
