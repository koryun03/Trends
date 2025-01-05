using System.Text.RegularExpressions;

namespace WebApplication121.Extensions
{
    public static class StringExtenion
    {
        public static string CleanString(this string input)
        {
            return Regex.Replace(input, @"\\", "");
        }
    }
    public static class Esim
    {
        public static string CleanStringg(string input)
        {
            return input.Replace("\\\"", "");

            // return input.Replace(a, "");
            //return Regex.Replace(input, @"\\", "");
        }
    }
}
