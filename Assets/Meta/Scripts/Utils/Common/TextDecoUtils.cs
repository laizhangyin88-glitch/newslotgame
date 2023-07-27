using System;
using System.Globalization;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class TextDecoUtils
    {
        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public enum TextFormat
        {
            UPPER = 0, // UPPER
            LOWER = 1, // lower
            CAMEL_CASE = 2, // camelCase
            PASCAL_CASE = 3, // PascalCase
        }

        public static string MsToTimeText(long ms) // todo fix
        {
            long sec = ms / 1000;
            long hour = sec / 3600L;
            sec -= hour * 3600L;
            long min = sec / 60L;
            sec -= min * 60L;

            string hourText = "", minText = "", secText = "";
            string unit;

            if (hour > 0)
            {
                unit = hour > 1 ? "HOURS_TEXT" : "HOUR_TEXT";
                hourText = string.Format("{0} {1}", hour, StringTableUtils.GetString(GLOBAL, unit));
            }

            if (min > 0)
            {
                unit = min > 1 ? "MINUTES_TEXT" : "MINUTE_TEXT";
                minText = string.Format("{0} {1}", min, StringTableUtils.GetString(GLOBAL, unit));
            }

            if(sec > 0)
            {
                unit = sec > 1 ? "SECONDS_TEXT" : "SECOND_TEXT";
                secText = string.Format("{0} {1}", sec, StringTableUtils.GetString(GLOBAL, unit));
            }

            return string.Format("{0}{1}{2}", hourText, minText, secText);
        }

        public static string EnumTypeToText<T>(int i, TextFormat format, string replaceUnderline = "_") where T : struct, Enum
        {
            bool isSuccess = Enum.TryParse(i.ToString(), out T value);
            if (!isSuccess) return string.Empty;

            string res = value.ToString();
            if (res == string.Empty || res.Length < 1) return res;

            TextInfo ti = new CultureInfo("en-US", false).TextInfo;

            res = res.Replace("_", replaceUnderline);

            switch (format)
            {
                case TextFormat.UPPER:
                    res = res.ToUpper();
                    break;
                case TextFormat.LOWER:
                    res = res.ToLower();
                    break;
                case TextFormat.CAMEL_CASE:
                    char first = char.ToLower(res[0]);
                    res = ti.ToTitleCase(res.ToLower()).Remove(0, 1).Insert(0, first.ToString());
                    break;
                case TextFormat.PASCAL_CASE:
                    res = ti.ToTitleCase(res.ToLower());
                    break;
            }

            return res;
        }

        public static string RatioToPercentage(float ratio, int digits = 0)
        {
            return Math.Round(ratio * 100f, digits) + "%";
        }

        public static string NumberToOrdinal(int number)
        {
            switch (number)
            {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                default:
                    return string.Format("{0}th", number);
            }
        }

        public static string GetColorFormatText(string text, Color color)
        {
            return string.Format("<color={0}>{1}</color>", color, text);
        }

        public static string GetColorFormatText(string text, Common.EColor color)
        {
            string tColor = EnumTypeToText<Common.EColor>((int)color, TextFormat.LOWER);
            return string.Format("<color={0}>{1}</color>", tColor, text);
        }

        public static bool ConvertStringFormat(ref string text, string format, string data)
        {
            if (text.Contains(format))
            {
                text = text.Replace(format, data);
                return true;
            }
            return false;
        }

        public static string ConvertCoinStyleText(long credit)
        {
            string coinText = FormatUtility.CommaNumberFormat(credit);
            coinText = "<style=coin>" + coinText + "</style>";
            return coinText;
        }

        public static string ConvertSimpleCoinStyleText(long credit)
        {
            if (credit == 0)
                return string.Empty;

            string coinText = FormatUtility.SimpleNumberFormat(credit);
            coinText = $"<style=coin>{coinText}</style>";
            return coinText;
        }
        
        public static string ConvertSimpleGemStyleText(long gem)
        {
            if (gem == 0)
                return string.Empty;

            string gemText = FormatUtility.SimpleNumberFormat(gem);
            gemText = $"<style=gem>{gemText}</style>";
            return gemText;
        }

        public static string ConvertAndSymbol(long a, long b)
        {
            if (a == 0 || b == 0)
            {
                return string.Empty;
            }
            else
            {
                return " & ";
            }
        }

        public static string ConvertTierStyleText(int tier)
        {
            int tierGroup = TierUtils.GetTierGroup(tier);
            string tierText = StringTableUtils.GetString(GLOBAL, string.Format("TIER_{0}", tier));
            string tierStyleText = StringTableUtils.GetString(GLOBAL, string.Format("TIER_STYLE_{0}", tierGroup), tierText);
            return tierStyleText;
        }

        public static string ConvertVipStyleText(long rp)
        {
            string rpText = FormatUtility.CommaNumberFormat(rp);
            rpText = "<style=vip>" + rpText + "</style>";
            return rpText;
        }
    }
}
