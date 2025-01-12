using System.Text.Json;
using WebApplication121.ServiceInterfaces;

namespace WebApplication121.Services
{
    public class TranslateService : ITranslateService
    {
        public /*static*/ async Task<string> TranslateToEnglishAsync(string text)
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
    }
}
