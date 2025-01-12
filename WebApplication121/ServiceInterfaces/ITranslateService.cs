namespace WebApplication121.ServiceInterfaces
{
    public interface ITranslateService
    {
        Task<string> TranslateToEnglishAsync(string text);
    }
}
