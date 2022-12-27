using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KOM.Scribere.Common
{
    public static class StringExtensions
    {
        public static string GetSearchText(string name)
        {
            // Append title
            var text = name.ToLower();

            // Remove all non-alphanumeric characters
            var regex = new Regex(@"[^\w\d]", RegexOptions.Compiled);
            text = regex.Replace(text, " ");

            var searchTerms = new List<string>();

            searchTerms.Add(text);
            searchTerms.Add(text.ConvertLatinToCyrillicLetters());

            // Combine all words
            return string.Join(" ", searchTerms);
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + input.Substring(1, input.Length - 1);
        }

        public static string ConvertLatinToCyrillicLetters(this string input)
        {
            var latinLetters = new[]
                               {
                                   "sht", "sh", "ck", "th", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                               };
            var cyrillicLetters = new[]
                                  {
                                      "щ", "ш", "к", "д", "а", "б", "ц", "д", "е", "ф", "г", "х", "и", "й", "к", "л", "м", "н", "о", "п", "кю", "р", "с", "т", "у", "в", "у", "кс", "я", "з",
                                  };

            for (var index = 0; index < latinLetters.Length; ++index)
            {
                input = input.Replace(latinLetters[index], cyrillicLetters[index]);
                input = input.Replace(latinLetters[index].CapitalizeFirstLetter(), cyrillicLetters[index].CapitalizeFirstLetter());
            }

            return input;
        }
    }
}
