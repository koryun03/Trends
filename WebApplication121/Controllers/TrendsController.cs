using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebApplication121.Models;

namespace WebApplication121.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrendsController : ControllerBase
    {

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData(string? url, /*string rowscount,*/ int? pageNumber)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var trendData = new List<TrendRow>();

            var options = new ChromeOptions();
            options.AddArgument("--headless"); //  aranc chrome GUI
            options.AddArgument("--disable-gpu");  //vor GUI chka esel petq chi
            //options.AddArgument("--no-sandbox"); //vkladkaneri isolation(ijacnuma security-n ete comment chanenq)

            using (IWebDriver driver = new ChromeDriver(options))
            {
                if (string.IsNullOrEmpty(url))
                {
                    url = "https://trends.google.com/trending?geo=US&hours=24";
                }

                driver.Navigate().GoToUrl(url);

                await Task.Delay(1000);
                //await Task.Delay(500);


                if (pageNumber > 0)
                {
                    try
                    {
                        var button = driver.FindElement(By.CssSelector("button[aria-label='Go to next page']"));
                        for (int i = 0; i < pageNumber; i++)
                        {
                            button.Click();
                            await Task.Delay(100);
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        return BadRequest("Button was not found.");
                    }
                }

                var rows = driver.FindElements(By.CssSelector("tbody tr"));

                //for (int i = 0; i < Math.Min(200, rows.Count); i++)
                for (int i = 1; i < rows.Count; i++)
                {
                    var row = rows[i];

                    var trendRow = new TrendRow { RowNumber = i };

                    //var columns = row.FindElements(By.CssSelector("td"));

                    try
                    {
                        string name = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.jvkLtd")).FindElement(By.CssSelector("div.mZ3RIc")).Text;
                        trendRow.Name = await TranslateToEnglishAsync(name);
                        trendRow.SearchVolume = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.dQOTjf")).FindElement(By.CssSelector("div.p6GDQc")).FindElement(By.CssSelector("div.lqv0Cb")).Text;
                        trendRow.Percent = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.dQOTjf")).FindElement(By.CssSelector("div.wqrjjc")).FindElement(By.CssSelector("div.TXt85b")).Text;
                        trendRow.StartedAt = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.WirRge")).FindElement(By.CssSelector("div.vdw3Ld")).Text;
                        trendRow.Status = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.WirRge")).FindElement(By.CssSelector("div.UQMqQd")).Text;
                        //     trendRow.TrendBreakdown = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.xm9Xec")).FindElement(By.CssSelector("div.k36WW")).Text;

                        var targetCell = row.FindElement(By.CssSelector("td.enOdEe-wZVHld-aOtOmf.oQ5Nq.wFXHce"));
                        var svgElement = targetCell.FindElement(By.CssSelector("svg"));
                        trendRow.SvgHtml = svgElement.GetAttribute("outerHTML");
                        //trendRow.SvgHtml = svgElement.GetAttribute("outerHTML").CleanString();
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("error");
                    }

                    trendData.Add(trendRow);
                }

                driver.Quit();
            }

            return Ok(trendData);
        }
        private static async Task<string> TranslateToEnglishAsync(string text)
        {
            const string ApiBaseUrl = "http://lingva.ml/api/v1";

            using var client = new HttpClient();

            // Кодируем текст для использования в URL
            string encodedText = Uri.EscapeDataString(text);

            // URL для перевода с автоопределением языка на английский
            string requestUrl = $"{ApiBaseUrl}/auto/en/{encodedText}";

            try
            {
                // Отправляем запрос
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                // Читаем ответ
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseBody);

                // Получаем переведённый текст
                string translatedText = jsonDocument.RootElement.GetProperty("translation").GetString();
                return translatedText;
            }
            catch (Exception ex)
            {
                return text;
            }
        }
    }
}