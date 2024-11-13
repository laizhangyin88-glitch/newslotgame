using UnityEngine;
using SlotMaker;
using System;

namespace BagelCode
{
    public class TimeUtils
    {
        public static long ONE_MIN_MS = 60000L;
        public static long ONE_HOUR_MS = 3600000L;
        public static long ONE_DAY_MS = 86400000L;

        public static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // private static long timeOffset  = 0;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public static void TimestampMSToDHMS(long timestampMS, out int day, out int hour, out int min, out int sec)
        {
            sec = (int)(timestampMS / 1000L);
            day = sec / 86400;
            sec -= day * 86400;
            hour = sec / 3600;
            sec -= hour * 3600;
            min = sec / 60;
            sec -= min * 60;
        }

        public static void TimestampMSToHMS(long timestampMS, out int hour, out int min, out int sec)
        {
            sec = (int)(timestampMS / 1000L);
            hour = sec / 3600;
            sec -= hour * 3600;
            min = sec / 60;
            sec -= min * 60;
        }

        public static string GetTimeTextDDHHMM(long days, long hours, long minutes)
        {
            return StringTableUtils.GetString(GLOBAL, "TIME_FORMAT_DDHHMM", days, hours, minutes);
        }

        public static string GetTimeTextHHMMSS(long hours, long minutes, long seconds)
        {
            return StringTableUtils.GetString(GLOBAL, "TIME_FORMAT_HHMMSS", 0, hours, minutes, seconds);
        }

        public static string GetTimeTextMMSS(long minutes, long seconds)
        {
            return StringTableUtils.GetString(GLOBAL, "TIME_FORMAT_MMSS", 0, 0, minutes, seconds);
        }

        public static string GetTimeTextHHMM(long hours, long minutes)
        {
            return StringTableUtils.GetString(GLOBAL, "TIME_FORMAT_HHMM", 0, hours, minutes);
        }

        public static long HourToSeconds(float hour)
        {
            float fSec = hour * 72000f;
            return (long)fSec;
        }

        public static int SecondsToDays(int seconds)
        {
            return seconds / 60 / 60 / 24;
        }

        public static long MilliSecondsToDay(long ms)
        {
            float factor = 0.0000000115740741f;
            return (long)(ms * factor);
        }

        public static long GetCurrentTime() => GetTimeStamp() / 1000L;

        public static long GetDeltaTimestamp() => (long)(Time.deltaTime * 1000f);

        public static long GetTimeStamp()
        {
            return (long)((System.DateTime.UtcNow - Jan1St1970).TotalMilliseconds); // todo : server sync offset
        }

        public static long GetCurrentLocalTime()
        {
            return (long)((System.DateTime.Now - Jan1St1970).TotalMilliseconds);
        }

        public static int GetTimeZoneOffset()
        {
            return TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
        }

        public static DateTime ParseTimestampToDateTime(long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(timestamp);
            return dateTime;
        }

        public static string GetPSTDateString(long timestamp, string dateFormat)
        {
            DateTime dateTime = ParseTimestampToDateTime(timestamp);
            DateTime pstDateTime = dateTime + new TimeSpan(-08, 00, 00);

            // Check the time in PDT
            // (PDT starts at 2 AM on the second Sunday in March each year, turn the clock 1 hour forward to 3 AM at that time.
            //  PDT ends at 2 AM on the first Sunday of November each year, turn the clock 1 hour back to 1 AM at that time.)
            int thisYear = pstDateTime.Year;
            DayOfWeek firstDayOfWeekOfMar = (new DateTime(thisYear, 3, 1, 0, 0, 0)).DayOfWeek;
            DayOfWeek firstDayOfWeekOfNov = (new DateTime(thisYear, 11, 1, 0, 0, 0)).DayOfWeek;
            // DayOfWeek.Sunday = 0 ~ DayOfWeek.Saturday = 6
            int secondSundayDayOfMar = (7 * 2 - (int)firstDayOfWeekOfMar) + 1;
            int firstSundayDayOfNov = (7 * 1 - (int)firstDayOfWeekOfNov) + 1;
            DateTime pdtStartDateTime = new DateTime(thisYear, 3, secondSundayDayOfMar, 2, 0, 0);
            DateTime pdtEndDateTime = new DateTime(thisYear, 11, firstSundayDayOfNov, 1, 0, 0);
            if (pdtStartDateTime <= pstDateTime && pstDateTime < pdtEndDateTime)
            {
                pstDateTime = pstDateTime + new TimeSpan(01, 00, 00);
            }

            return pstDateTime.ToString(dateFormat);
        }

        public static DateTime ParseTimestampToLocalDateTime(long timestamp)
        {
            DateTime dateTime = ParseTimestampToDateTime(timestamp);

            return dateTime.ToLocalTime();
        }

        public static DateTime GetLocalDateTime()
        {
            return ParseTimestampToLocalDateTime(GetTimeStamp());
        }

        public static DateTime GetCurrentDateTime()
        {
            return ParseTimestampToDateTime(GetTimeStamp());
        }

        // session play time
        private static long totalSessionPlayTime;
        private static float sessionStartTime;

        public static void UpdateSessionBeginTime()
        {
            sessionStartTime = Time.time;
        }

        public static void UpdateTotalSessionTime(long totalSessionTime)
        {
            totalSessionPlayTime = totalSessionTime;
        }

        public static long GetSessionPlayTime()
        {
            return (long)((Time.time - sessionStartTime) * 1000f);
        }

        public static long GetTotalSessionPlayTime()
        {
            return totalSessionPlayTime + GetSessionPlayTime();
        }

        public static void AccmulateSessionPlayTime() { totalSessionPlayTime += GetSessionPlayTime(); }

        public static TimeSpan GetLastLoginTimeOffset(long lastLoginTimestamp)
        {
            DateTime lastLoginDateTime = ParseTimestampToLocalDateTime(lastLoginTimestamp);
            TimeSpan timeOffset = GetLocalDateTime() - lastLoginDateTime;
            return timeOffset;
        }

        public static int GetLastLoginDayCount(long lastLoginTimestamp)
        {
            DateTime lastLoginDateTime = ParseTimestampToLocalDateTime(lastLoginTimestamp);
            TimeSpan timeOffset = GetLocalDateTime() - lastLoginDateTime;
            return timeOffset.Days;
        }

        public static long ApplyTimeZoneOffset(long timestamp)
        {
            if (timestamp == 0) return timestamp;

            DateTime dateTime = ParseTimestampToDateTime(timestamp);
            dateTime = dateTime.AddHours(-GetTimeZoneOffset());
            return (long)(dateTime - Jan1St1970).TotalMilliseconds;
        }

        public static long GetNextDayTimestamp(bool isLocal, int hourOffset = 0)
        {
            DateTime currentDate = ParseTimestampToDateTime(GetTimeStamp());

            currentDate = currentDate.AddDays(1);
            currentDate = currentDate.AddHours(hourOffset);

            if (isLocal)
                return ApplyTimeZoneOffset((long)((currentDate - Jan1St1970).TotalMilliseconds));
            return (long)((currentDate - Jan1St1970).TotalMilliseconds);
        }

        public static bool IsAvailableTimestamp(long startTimestamp, long endTimestamp)
        {
            long currentTimestamp = GetTimeStamp();
            return currentTimestamp >= startTimestamp && currentTimestamp <= endTimestamp;
        }

        public static string GetRemainingTimeText(long endTimestamp, string textKey, long warningTimestamp = 0L, string textFormat = "", string warningTextFormat = "", bool useCommonTimer = true)
        {
            if (string.IsNullOrEmpty(textKey)) return "";

            long currentTimestamp = GetTimeStamp();
            DateTime endDateTime = TimeUtils.ParseTimestampToDateTime(endTimestamp);
            DateTime currentDateTime = TimeUtils.ParseTimestampToDateTime(currentTimestamp);

            TimeSpan leftTimeSpan = endDateTime - currentDateTime;

            long remainingTimestamp = (long)leftTimeSpan.TotalMilliseconds;
            if (remainingTimestamp <= 0)
                return StringTableUtils.GetString(GLOBAL, textKey, 0, 0, 0, 0, 0);

            string resultText = "";
            if (useCommonTimer && leftTimeSpan.TotalDays >= 1.0)
            {
                resultText = StringTableUtils.GetString(GLOBAL, "TIME_FORMAT_COMMON_TIME_LEFT", (int)leftTimeSpan.TotalDays);
            }
            else
            {
                resultText = StringTableUtils.GetString(GLOBAL, textKey, leftTimeSpan.Days, leftTimeSpan.Hours,
                    leftTimeSpan.Minutes, leftTimeSpan.Seconds, Math.Truncate((double)leftTimeSpan.TotalHours));
            }

            if (warningTimestamp > 0L && currentTimestamp < warningTimestamp && !string.IsNullOrEmpty(warningTextFormat))
            {
                resultText = StringTableUtils.GetString(GLOBAL, warningTextFormat, resultText);
            }
            else if (!string.IsNullOrEmpty(textFormat))
            {
                resultText = StringTableUtils.GetString(GLOBAL, textFormat, resultText);
            }

            return resultText;
        }
    }
}
