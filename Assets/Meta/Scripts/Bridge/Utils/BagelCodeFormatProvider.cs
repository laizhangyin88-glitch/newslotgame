using System;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    // usage example
    //     Debug.Log(string.Format(new BagelCodeFormatProvider(),
    // "You have {0:Plural;freeSpin} left, {1:Ordinal;Capitalize} and {2:LimitedString}. You also have {3:CommaNumber} credits",
    // 1, 2, "kkkkkkkkkkkkk", 1000000000));

    public class BagelCodeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const char SPLIT_CHARACTER = ';';
        private const string ARG0_FORMAT = "{0}";
        private const string EMPTY_OPTION = "";
        private const string UPPER_CASE = "Upper";
        private const string CAPITALIZER_CASE = "Capitalize";
        private const string WITHOUT_VALUE = "WithoutValue";
        private static readonly string[,] ORDINAL_POSTFIX = new string[,] {
            { "th", "st", "nd", "rd" },
            { "TH", "ST", "ND", "RD" },
            { "Th", "St", "Nd", "Rd" }
        };
        private const int LIMITED_STRING_DEFAULT_LENGTH = 8;
        private const string ABBREVIATED_POSTFIX = "..";
        private static readonly string[] TIER = new string[] {
            "TIER_0",
            "TIER_1",
            "TIER_2",
            "TIER_3",
            "TIER_4",
            "TIER_5",
            "TIER_6",
            "TIER_7",
            "TIER_8",
            "TIER_9",
            "TIER_10",
            "TIER_11",
            "TIER_12",
            "TIER_13",
            "TIER_14",
            "TIER_15",
            "TIER_16",
            "TIER_17",
            "TIER_18",
            "TIER_19",
            "TIER_20"
        };
        private static readonly string[] TIER_STYLE_NAME = new string[] {
            "TIER_STYLE_NAME_0",
            "TIER_STYLE_NAME_1",
            "TIER_STYLE_NAME_2",
            "TIER_STYLE_NAME_3",
            "TIER_STYLE_NAME_4",
            "TIER_STYLE_NAME_5",
            "TIER_STYLE_NAME_6"
        };
        private static readonly string[] TIER_STYLE = new string[] {
            "TIER_STYLE_0",
            "TIER_STYLE_1",
            "TIER_STYLE_2",
            "TIER_STYLE_3",
            "TIER_STYLE_4",
            "TIER_STYLE_5",
            "TIER_STYLE_6"
        };
        private static readonly string[] TIER_SPRITE = new string[] {
            "TIER_SPRITE_0",
            "TIER_SPRITE_1",
            "TIER_SPRITE_2",
            "TIER_SPRITE_3",
            "TIER_SPRITE_4",
            "TIER_SPRITE_5",
            "TIER_SPRITE_6"
        };
        private const string MULTIPLIER_FORMAT = "#,##0.#";
        private static readonly char[] MULTIPLIER_CHARACTER = { 'x', 'X' };
        private const string REAR = "Rear";
        private const string MAXIMUM_BADGE_NUMBER = "99+";
        private const string GAME_TITLE = "GAME_TITLE_";
        private const string SYMBOL_NAME = "SYMBOL_NAME_";
        private const char SPACE = ' ';
        private const string DATE_DAYS = "days";
        private const string DATE_DAY = "1 day";
        private const string DATE_HOURS = "hours";
        private const string DATE_HOUR = "1 hour";
        private const string DATE_MINS = "mins";
        private const string DATE_MIN = "1 min";
        private const string SOON = "soon";
        private const int DAY_SECONDS = 86400;
        private const int HOUR_SECONDS = 3600;
        private const int MINUTE_SECONDS = 60;
        private const string LAST_DAYS = "LAST_DAYS_";
        private const string LAST_HOURS = "LAST_HOURS";
        private const string LAST_MINS  = "LAST_MINS";
        private const string LAST_NOW   = "LAST_JUSTNOW";
        private const string DATE_OVERTEN = "+ 10";
        private static readonly long[] CREDIT_RANGE = new long[] {
            1000000,
            10000000,
            100000000,
            1000000000
        };
        private static readonly string[] CREDIT_COLOR = new string[] {
            "CREDIT_COLOR_0",
            "CREDIT_COLOR_1",
            "CREDIT_COLOR_2",
            "CREDIT_COLOR_3",
            "CREDIT_COLOR_4"
        };
        private static readonly string[] CREDIT_SIMPLE_COLOR = new string[] {
            "CREDIT_SIMPLE_COLOR_0",
            "CREDIT_SIMPLE_COLOR_1",
            "CREDIT_SIMPLE_COLOR_2",
            "CREDIT_SIMPLE_COLOR_3",
            "CREDIT_SIMPLE_COLOR_4"
        };

        private static readonly string[] CLUB_TIER = new string[] {
            "CLUB_TIER_0",
            "CLUB_TIER_1",
            "CLUB_TIER_2",
            "CLUB_TIER_3",
            "CLUB_TIER_4",
            "CLUB_TIER_5",
            "CLUB_TIER_6",
            "CLUB_TIER_7",
            "CLUB_TIER_8",
            "CLUB_TIER_9",
            "CLUB_TIER_10",
            "CLUB_TIER_11"
        };

        private static readonly string[] CLUB_TIER_STYLE = new string[] {
            "CLUB_TIER_STYLE_0",
            "CLUB_TIER_STYLE_1",
            "CLUB_TIER_STYLE_2",
            "CLUB_TIER_STYLE_3",
            "CLUB_TIER_STYLE_4",
            "CLUB_TIER_STYLE_5"
        };
        private static readonly string[] CLUB_TIER_SPRITE = new string[] {
            "CLUB_TIER_SPRITE_0",
            "CLUB_TIER_SPRITE_1",
            "CLUB_TIER_SPRITE_2",
            "CLUB_TIER_SPRITE_3",
            "CLUB_TIER_SPRITE_4",
            "CLUB_TIER_SPRITE_5",
            "CLUB_TIER_SPRITE_6",
            "CLUB_TIER_SPRITE_7",
            "CLUB_TIER_SPRITE_8",
            "CLUB_TIER_SPRITE_9",
            "CLUB_TIER_SPRITE_10",
            "CLUB_TIER_SPRITE_11"
        };

        private readonly StringBuilder cachedStringBuilder = new StringBuilder(256);

        private Dictionary<string, Func<string[], object, IFormatProvider, string>> funcDict = new Dictionary<string, Func<string[], object, IFormatProvider, string>>();

        public BagelCodeFormatProvider()
        {
            funcDict["Plural"] = Plural;
            funcDict["PluralState"] = PluralState;
            funcDict["Ordinal"] = Ordinal;
            funcDict["Upper"] = Upper;
            funcDict["Lower"] = Lower;
            funcDict["SimpleNumber"] = SimpleNumber;
            funcDict["SimpleSignificantNumber"] = SimpleSignificantNumber;
            funcDict["CommaNumber"] = CommaNumber;
            funcDict["KiloNumber"] = KiloNumber;
            funcDict["MillionNumber"] = MillionNumber;
            funcDict["BillionNumber"] = BillionNumber;
            funcDict["AutoNumber"] = AutoNumber;
            funcDict["VerticalNumber"] = VerticalNumber;
            funcDict["VerticalSimpleNumber"] = VerticalSimpleNumber;
            funcDict["NumberUnit"] = NumberUnit;
            funcDict["LimitedString"] = LimitedString;
            funcDict["TierIcon"] = TierIcon;
            funcDict["TierSpriteText"] = TierSpriteText;
            funcDict["TierStyleText"] = TierStyleText;
            funcDict["TierStyleName"] = TierStyleName;
            funcDict["TierName"] = TierName;
            funcDict["Multiplier"] = Multiplier;
            funcDict["BadgeNumber"] = BadgeNumber;
            funcDict["Content"] = ContentName;
            funcDict["Symbol"] = SymbolName;
            funcDict["ISO8601"] = ISO8601;
            funcDict["TimestampToPeriod"] = TimestampToPeriod;
            funcDict["DateInterval"] = DateInterval;
            funcDict["LastLoginDays"] = LastLoginDays;
            funcDict["LastHours"] = LastHours;
            funcDict["CreditColor"] = CreditColor;
            funcDict["SimpleCreditColorNumber"] = SimpleCreditColorNumber;
            funcDict["ClubTierName"] = ClubTierName;
            funcDict["ClubTierSpriteText"] = ClubTierSpriteText;
            funcDict["ClubTierStyleText"] = ClubTierStyleText;
            funcDict["ClubTierSprite"] = ClubTierSprite;
            funcDict["SymbolizedSeconds"] = SymbolizedSeconds;
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return arg.ToString();

            string[] formatList = format.Split(SPLIT_CHARACTER);

            Func<string[], object, IFormatProvider, string> func = null;
            if (funcDict.TryGetValue(formatList[0], out func))
                return func(formatList, arg, formatProvider);

            return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
        }

        // {0:Plural;STR_FREESPIN_SINGULAR;STR_INGAME_FREESPIN_PLURAL}
        public string Plural(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            cachedStringBuilder.Length = 0;
            cachedStringBuilder.Append(value);

            if (value == 1)
            {
                cachedStringBuilder.Append(formatList[1]);
            }
            else
            {
                cachedStringBuilder.Append(formatList[2]);
            }

            return cachedStringBuilder.ToString();
        }

        public string PluralState(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            if (value == 1)
            {
                return formatList[1];
            }

            return formatList[2];
        }

        // {0:Ordinal;Upper}
        // {0:Ordinal;Capitalize}
        // {0:Ordinal;WithoutValue}
        public string Ordinal(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);
            int option = 0;
            int postfixType = 0;
            bool withoutValue = false;

            int hundredNum = value % 100;

            if(hundredNum > 0 && !(hundredNum >= 11 && hundredNum <= 13))
            {
                int num = hundredNum % 10;
                if (num > 0 && num < 4)
                    postfixType = num;
            }

            if (formatList.Length > 1)
            {
                for(int i=1; i<formatList.Length ; ++i)
                {
                    if (formatList[i].Equals(UPPER_CASE, StringComparison.Ordinal))
                        option = 1;
                    else if (formatList[i].Equals(CAPITALIZER_CASE, StringComparison.Ordinal))
                        option = 2;
                    else if (formatList[i].Equals(WITHOUT_VALUE, StringComparison.Ordinal))
                        withoutValue = true;
                }
            }

            cachedStringBuilder.Length = 0;
            if(!withoutValue)
                cachedStringBuilder.Append(value);

            cachedStringBuilder.Append(ORDINAL_POSTFIX[option, postfixType]);
            return cachedStringBuilder.ToString();
        }

        public string Upper(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return ((string)arg).ToUpper();
        }

        public string Lower(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return ((string)arg).ToLower();
        }

        // {0:SimpleNumber}
        public string SimpleNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormatUtility.SimpleNumberFormat(Convert.ToInt64(arg));
        }

        // {0:SimpleSignificantNumber}
        public string SimpleSignificantNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long number = Convert.ToInt64(arg);

            long max = FormatUtility._10K;

            if (formatList.Length > 1)
            {
                long format = Convert.ToInt64(formatList[1]);
                if (format > 0L) max = format;
            }

            if (number < max)
                return FormatUtility.CommaNumberFormat(number);
            else if (number < FormatUtility._100K)
                return string.Format("{0:##.#}K", ((double)number / FormatUtility._1K) - 0.05);
            else if (number < FormatUtility._1M)
                return string.Format("{0:###}K", (number / FormatUtility._1K));
            else if (number < FormatUtility._10M)
                return string.Format("{0:#.##}M", ((double)number / FormatUtility._1M) - 0.005);
            else if (number < FormatUtility._100M)
                return string.Format("{0:##.#}M", ((double)number / FormatUtility._1M) - 0.05);
            else if (number < FormatUtility._1B)
                return string.Format("{0:###}M", number / FormatUtility._1M);
            else if (number < FormatUtility._10B)
                return string.Format("{0:#.##}B", ((double)number / FormatUtility._1B) - 0.005);
            else if (number < FormatUtility._100B)
                return string.Format("{0:##.#}B", ((double)number / FormatUtility._1B) - 0.05);

            return string.Format("{0:###}B", number / FormatUtility._1B);
        }

        // {0:CommaNumber}
        public string CommaNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormatUtility.CommaNumberFormat(Convert.ToInt64(arg));
        }

        // {0:KiloNumber}
        public string KiloNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormattedNumber(Convert.ToInt64(arg), FormatUtility._1K, "K");
        }

        // {0:MillionNumber}
        public string MillionNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormattedNumber(Convert.ToInt64(arg), FormatUtility._1M, "M");
        }

        // {0:BillionNumber}
        public string BillionNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormattedNumber(Convert.ToInt64(arg), FormatUtility._1B, "B");
        }

        private string FormattedNumber(long number, long unit, string unitText)
        {
            if (number < unit)
            {
                return string.Format("{0:0.00}" + unitText, (double)number / unit);
            }
            else
            {
                return string.Format("{0:N0}" + unitText, number / unit);
            }
        }

        public string AutoNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long value = Convert.ToInt64(arg);

            long threshold = Convert.ToInt64(formatList[1]);
            if (value >= threshold)
                return FormatUtility.SimpleNumberFormat(value);

            return FormatUtility.CommaNumberFormat(value);
        }

        public string VerticalSimpleNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            string value = FormatUtility.SimpleNumberFormat(Convert.ToInt64(arg));
            string str = "";
            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] == '.' || value[i] == ',')
                    str = str.Remove(str.Length - 2);
                str += value[i] + "\r\n";
            }
            str = str.Remove(str.Length - 2);

            if (value[1] == '.' || value[1] == ',')
                str = str.Insert(0, " ");
            else if(value[2] == '.')
                str = str.Insert(3, " ");

            return str;
        }

        public string VerticalNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            string value = Convert.ToString(arg);
            string str = "";
            for (int i = 0; i < value.Length; ++i)
            {
                str += value[i] + "\r\n";
            }
            return str;
        }

        public string NumberUnit(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            return FormatUtility.NumberUnit(Convert.ToInt64(arg));
        }

        // {0:LimitedString}
        // {0:LimitedString;3}
        public string LimitedString(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            string value = (string)arg;
            int limitLength = LIMITED_STRING_DEFAULT_LENGTH;
            if (formatList.Length > 1)
                limitLength = Convert.ToInt32(formatList[1]);

            if (value.Length <= limitLength)
                return value;

            cachedStringBuilder.Length = 0;
            cachedStringBuilder.Append(value.Substring(0, limitLength - 1));
            cachedStringBuilder.Append(ABBREVIATED_POSTFIX);
            return cachedStringBuilder.ToString();
        }

        // {0:TierIcon}
        public string TierIcon(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            int tierGroup = TierUtils.GetTierGroup(value);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_SPRITE[tierGroup], "");
        }

        // {0:TierSpriteText}
        public string TierSpriteText(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            int tierGroup = TierUtils.GetTierGroup(value);
            string tierText = StringTableUtils.GetString(StringTable.StringTableType.Global, TIER[value]);
            string tierStyle = StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_STYLE[tierGroup], tierText);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_SPRITE[tierGroup], tierStyle);
        }

        // {0:TierStyleText}
        public string TierStyleText(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            int tierGroup = TierUtils.GetTierGroup(value);
            string tierText = StringTableUtils.GetString(StringTable.StringTableType.Global, TIER[value]);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_STYLE[tierGroup], tierText);
        }

        // {0:TierStyleName}
        public string TierStyleName(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);
            int tierGroup = TierUtils.GetTierGroup(value);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_STYLE_NAME[tierGroup]);
        }

        // {0:TierName}
        public string TierName(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            // int tierGroup = TierUtils.GetTierGroup(value);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, TIER[value]);
        }

        // {0:Multiplier}
        public string Multiplier(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            float value = Convert.ToSingle(arg);

            bool rear = false;
            int upper = 0;
            int count = formatList.Length;
            for (int i = 1; i < count; ++i)
            {
                if (formatList[i].Equals(REAR, StringComparison.Ordinal))
                    rear = true;
                else if (formatList[i].Equals(UPPER_CASE, StringComparison.Ordinal))
                    upper = 1;
            }

            cachedStringBuilder.Length = 0;
            if (rear)
            {
                cachedStringBuilder.Append(((IFormattable)value).ToString(MULTIPLIER_FORMAT, CultureInfo.CurrentCulture));
                cachedStringBuilder.Append(MULTIPLIER_CHARACTER[upper]);
            }
            else
            {
                cachedStringBuilder.Append(MULTIPLIER_CHARACTER[upper]);
                cachedStringBuilder.Append(((IFormattable)value).ToString(MULTIPLIER_FORMAT, CultureInfo.CurrentCulture));
            }
            return cachedStringBuilder.ToString();
        }

        public string BadgeNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = Convert.ToInt32(arg);

            if (value > 99)
                return MAXIMUM_BADGE_NUMBER;

            return value.ToString();
        }

        public string ContentName(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            cachedStringBuilder.Length = 0;
            cachedStringBuilder.Append(GAME_TITLE);
            cachedStringBuilder.Append(value);

            bool error = true;
            string returnValue = StringTableUtils.GetString(StringTable.StringTableType.Global, cachedStringBuilder.ToString(), out error);

            if (formatList.Length > 1 && formatList[1].Equals(UPPER_CASE, StringComparison.Ordinal))
                return returnValue.ToUpper();

            return returnValue;
        }

        public string SymbolName(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            cachedStringBuilder.Length = 0;
            cachedStringBuilder.Append(SYMBOL_NAME);
            cachedStringBuilder.Append(value);

            bool error = true;
            string returnValue = StringTableUtils.GetString(StringTable.StringTableType.Content, cachedStringBuilder.ToString(), out error);

            if (formatList.Length > 1 && formatList[1].Equals(UPPER_CASE, StringComparison.Ordinal))
                return returnValue.ToUpper();

            return returnValue;
        }

        // 00:00:00
        public string ISO8601(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            // todo hour가 24 넘는 경우 처리

            long allTimeSeconds = Convert.ToInt64(arg);
            TimeUtils.TimestampMSToHMS(allTimeSeconds * 1000L, out int hour, out int min, out int sec);

            return string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
        }

        // 610 -> 10M10S
        // format:max symbol count
        public string SymbolizedSeconds(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int allTimeSeconds = Convert.ToInt32(arg);
            TimeUtils.TimestampMSToDHMS(allTimeSeconds * 1000L, out int day, out int hour, out int min, out int sec);

            string[] texts = new string[4]
            {
                day.ToString() + "D",
                hour.ToString() + "H",
                min.ToString() + "M",
                sec.ToString() + "S"
            };

            int i = (day == 0) ? (hour == 0) ? (min == 0) ? 3 : 2 : 1 : 0;

            int maxCountFormatNumber = 1;
            if (formatList.Length > 1)
                maxCountFormatNumber = Mathf.Max(Convert.ToInt32(formatList[1]), 1);

            int symbolCount = Math.Min(maxCountFormatNumber, 4 - i);

            string result = "";

            for(int j = 0; j < symbolCount; ++j)
                result += texts[i++];

            return result;
        }

        // MM/DD/YYYY
        public string TimestampToPeriod(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long remainingTS = (long)arg;
            var until = TimeUtils.ParseTimestampToDateTime(remainingTS);

            int month = until.Month;
            int day = until.Day;
            int year = until.Year;

            string divider = "/";
            if (formatList != null && formatList.Length > 1)
            {
                divider = formatList[1];
            }

            return string.Format("{0}{1}{2}{3}{4}", month, divider, day, divider, year);
        }

        //milliseconds timestamp -> day / hour / min
        public string DateInterval(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long value = Convert.ToInt64(arg) / 1000;

            cachedStringBuilder.Length = 0;

            long day = value / DAY_SECONDS;
            if (day > 1)
            {
                cachedStringBuilder.Append(day);
                cachedStringBuilder.Append(SPACE);
                cachedStringBuilder.Append(DATE_DAYS);
            }
            else if (day == 1)
            {
                cachedStringBuilder.Append(DATE_DAY);
            }
            else
            {
                long hour = value / HOUR_SECONDS;
                if (hour > 1)
                {
                    cachedStringBuilder.Append(hour);
                    cachedStringBuilder.Append(SPACE);
                    cachedStringBuilder.Append(DATE_HOURS);
                }
                else if (hour == 1)
                {
                    cachedStringBuilder.Append(DATE_HOUR);
                }
                else
                {
                    long minute = value / MINUTE_SECONDS;
                    if (minute > 1)
                    {
                        cachedStringBuilder.Append(minute);
                        cachedStringBuilder.Append(SPACE);
                        cachedStringBuilder.Append(DATE_MINS);
                    }
                    else if (minute == 1)
                    {
                        cachedStringBuilder.Append(DATE_MIN);
                    }
                    else
                    {
                        cachedStringBuilder.Append("< " + DATE_MIN);
                    }
                }
            }
            return cachedStringBuilder.ToString();
        }

        public string LastLoginDays(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);
            bool error = true;

            if (value == 0 || value == 1)
            {
                cachedStringBuilder.Length = 0;
                cachedStringBuilder.Append(LAST_DAYS);
                cachedStringBuilder.Append(value);

                return StringTableUtils.GetString(StringTable.StringTableType.Global, cachedStringBuilder.ToString(), out error);
            }
            else if (value < 11)
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_DAYS, value, out error);
            }
            else
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_DAYS, DATE_OVERTEN, out error);
            }
        }

        public string LastHours(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long value = (BagelCode.TimeUtils.GetTimeStamp() - Convert.ToInt64(arg))/1000L;

            int hour = (int)(value / HOUR_SECONDS);
            bool error = true;

            if (hour > 0)
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_HOURS, hour, out error);
            }
            else
            {
                long minute = value / MINUTE_SECONDS;

                if (minute > 5)
                {
                    if (minute % 5 > 0)
                    {
                        minute += (5 - minute % 5);

                        if (minute >= 60)
                        {
                            return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_HOURS, 1, out error);
                        }
                    }

                    return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_MINS, minute, out error);
                }
                else
                {
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, LAST_NOW, out error);
                }
            }
        }

        // {0:CreditColor}
        public string CreditColor(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long credit = System.Convert.ToInt64(arg);

            bool error = false;
            for (int i = 0; i < 4; ++i)
            {
                if (credit < CREDIT_RANGE[i])
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, CREDIT_COLOR[i], credit, out error);
            }

            return StringTableUtils.GetString(StringTable.StringTableType.Global, CREDIT_COLOR[CREDIT_COLOR.Length - 1], credit, out error);
        }

        // {0:SimpleCreditColorNumber}
        public string SimpleCreditColorNumber(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            long credit = System.Convert.ToInt64(arg);

            bool error = false;
            for (int i = 0; i < 4; ++i)
            {
                if (credit < CREDIT_RANGE[i])
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, CREDIT_SIMPLE_COLOR[i], credit, out error);
            }

            return StringTableUtils.GetString(StringTable.StringTableType.Global, CREDIT_SIMPLE_COLOR[CREDIT_SIMPLE_COLOR.Length - 1], credit, out error);
        }

        // {0:ClubTierName}
        public string ClubTierName(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            bool error = false;
            return StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER[value], out error);
        }

        // {0:ClubTierSpriteText}
        public string ClubTierSpriteText(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            bool error = false;
            int clubTierGroup = ClubUtils.GetClubTierGroup(value);
            string tierText = StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER[value], out error);
            string tierStyle = StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER_STYLE[clubTierGroup], tierText, out error);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER_SPRITE[value], tierStyle, out error);
        }

        // {0:ClubTierStyleText}
        public string ClubTierStyleText(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            bool error = false;
            int clubTierGroup = ClubUtils.GetClubTierGroup(value);
            string tierText = StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER[value], out error);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER_STYLE[clubTierGroup], tierText, out error);
        }

        // {0:ClubTierSprite} - Non tier style
        public string ClubTierSprite(string[] formatList, object arg, IFormatProvider formatProvider)
        {
            int value = System.Convert.ToInt32(arg);

            bool error = false;
            string tierText = StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER[value], out error);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, CLUB_TIER_SPRITE[value], tierText, out error);
        }
    }
}
