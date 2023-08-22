using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxTest
{
    public static class Util
    {
        public const string USER_ID = "2C106A22-4661-4EB2-AA5A-ED3D3A544FB6";
        public const string INVALID_USER_ID = "2C106A22-4661-4EB2-AA5A-ED3D5A534FB1";
        public static string ConvertToEnglishCharacters(string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
