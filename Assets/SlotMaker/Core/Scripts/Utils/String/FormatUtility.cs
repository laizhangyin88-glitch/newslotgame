using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SlotMaker
{
    public class FormatUtility
    {
        private const string FAST_COMMA_NUMBER_FORMAT = "N0";
        private const string COMMA_NUMBER_FORMAT = "{0:#,###0}";

        private const string _10K_FORMAT    = "00.0K";
        private const string _100K_FORMAT   = "000K";
        private const string _1M_FORMAT     = "0.00M";
        private const string _10M_FORMAT    = "00.0M";
        private const string _100M_FORMAT   = "000M";
        private const string _1B_FORMAT     = "0.00B";
        private const string _10B_FORMAT    = "00.0B";
        private const string _100B_FORMAT   = "000B";
        private const string _1T_FORMAT     = "0.00T";
        private const string _10T_FORMAT    = "00.0T";
        private const string _100T_FORMAT   = "#,000T";

        private const string _1K_UNIT = "K";
        private const string _1M_UNIT = "M";
        private const string _1B_UNIT = "B";
        private const string _1T_UNIT = "T";

        public const long ZeroCredit = 0;
        public const long OneCredit  = 1;
        public const long _1K        = 1000;
        public const long _10K       = 10000;
        public const long _100K      = 100000;
        public const long _1M        = 1000000;
        public const long _10M       = 10000000;
        public const long _100M      = 100000000;
        public const long _1B        = 1000000000;
        public const long _10B       = 10000000000;
        public const long _100B      = 100000000000;
        public const long _1T        = 1000000000000;
        public const long _10T       = 10000000000000;
        public const long _100T      = 100000000000000;

        private const string EQUAL_TO = "==";
        private const string NOT_EQUAL_TO = "!=";
        private const string GREATER_THAN = ">";
        private const string LESS_THAN = "<";
        private const string GREATER_OR_EQUAL_TO = ">=";
        private const string LESS_OR_EQUAL_TO = "<=";

        public static string CommaNumberFormat(double number)
        {
            return CommaNumberFormat((long)number);
        }

        public static string CommaNumberFormat(long number)
        {
            return number.ToString(FAST_COMMA_NUMBER_FORMAT);
        }

        public static string SimpleNumberFormat(int number)
        {
            return SimpleNumberFormat((long)number);
        }

        public static string SimpleNumberFormat(long number)
        {
            // if (number < _10K)
            //     return CommaNumberFormat(number);
            // else if (number < _100K)
            //     return ((double)number / _1K).ToString(_10K_FORMAT);
            // else if (number < _1M)
            //     return ((double)number / _1K).ToString(_100K_FORMAT);
            // else if (number < _10M)
            //     return ((double)number / _1M).ToString(_1M_FORMAT);
            // else if (number < _100M)
            //     return ((double)number / _1M).ToString(_10M_FORMAT);
            // else if (number < _1B)
            //     return ((double)number / _1M).ToString(_100M_FORMAT);
            // else if (number < _10B)
            //     return ((double)number / _1B).ToString(_1B_FORMAT);
            // else if (number < _100B)
            //     return ((double)number / _1B).ToString(_10B_FORMAT);

            // return ((double)number / _1B).ToString(_100B_FORMAT);

            if (number < _10K)
                return CommaNumberFormat(number);
            else if (number < _100K)
                return (((double)number / _1K) - 0.05).ToString(_10K_FORMAT);
            else if (number < _1M)
                return (number / _1K).ToString(_100K_FORMAT);
            else if (number < _10M)
                return (((double)number / _1M) - 0.005).ToString(_1M_FORMAT);
            else if (number < _100M)
                return (((double)number / _1M) - 0.05).ToString(_10M_FORMAT);
            else if (number < _1B)
                return (number / _1M).ToString(_100M_FORMAT);
            else if (number < _10B)
                return (((double)number / _1B) - 0.005).ToString(_1B_FORMAT);
            else if (number < _100B)
                return (((double)number / _1B) - 0.05).ToString(_10B_FORMAT);
            else if (number < _1T)
                return (number / _1B).ToString(_100B_FORMAT);
            else if (number < _10T)
                return (((double)number / _1T) - 0.005).ToString(_1T_FORMAT);
            else if (number < _100T)
                return (((double)number / _1T) - 0.05).ToString(_10T_FORMAT);

            return (number / _1T).ToString(_100T_FORMAT);
        }

        public static string NumberUnit(long number)
        {
            if (number < _1K)
                return string.Empty;
            else if (number < _1M)
                return _1K_UNIT;
            else if (number < _1B)
                return _1M_UNIT;
            else if (number < _1T)
                return _1B_UNIT;
            return _1T_UNIT;
        }

        public static bool CompareOperator<T>(string op, T left, T right) where T : System.IComparable<T> {
            switch (op) {
                case LESS_THAN: return left.CompareTo(right) < 0;
                case GREATER_THAN: return left.CompareTo(right) > 0;
                case LESS_OR_EQUAL_TO: return left.CompareTo(right) <= 0;
                case GREATER_OR_EQUAL_TO: return left.CompareTo(right) >= 0;
                case EQUAL_TO: return left.Equals(right);
                case NOT_EQUAL_TO: return !left.Equals(right);
                default: return false;
            }
        }

        public static Color GetColor(int red, int green, int blue, int alpha=255)
        {
            return new Color((float)red/255f, (float)green/255f, (float)blue/255f, (float)alpha/255f);
        }

        public static Color GetColor(string hexValue)
        {
            if(string.IsNullOrEmpty(hexValue)) return Color.white;

            int red     = Int32.Parse(hexValue.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            int green   = Int32.Parse(hexValue.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            int blue    = Int32.Parse(hexValue.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

            return GetColor(red, green, blue);
        }
    }
}
