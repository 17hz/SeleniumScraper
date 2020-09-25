using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Common.Utilities.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string HtmlDecode(this String str)
        {
            return Uri.UnescapeDataString(WebUtility.HtmlDecode(str));
        }

        public static string TrimSpecialChars(this String str)
        {
            return str.Trim('\n', '\r', '\t', ' ');
        }

        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}