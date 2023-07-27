using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BagelCode
{
    public static class StringUtility
    {
        public static bool IsNullOrWhiteSpace(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }

        /// <summary>
        /// Eg. ["Item1","Item2","Item3"] , "," => "Item1,Item2,Item3"
        /// </summary>
        public static string ToSeparatedString(this IEnumerable enumerable, string separator)
        {
            return string.Join(separator, enumerable.Cast<object>().Select(o => o?.ToString() ?? "(null)").ToArray());
        }

        public static string RemoveTrimAndNewline(string s)
        {
            if(string.IsNullOrEmpty(s)) return "";
            
            return Regex.Replace(s.Trim(), @"\t|\n|\r", "");
        }

        public static string UnFormatColorTags(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            str = Regex.Replace(str, "(?:<color=)(.*?)(?:>)", "");
            str = Regex.Replace(str, "</color>", "");
            return str;
        }
    }
}