using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class GetClubLeagueRemainningTime : ActionTask<Blackboard>
    {
        public BBParameter<long>    targetTime;

        public BBParameter<long>    saveLeftTimestamp;
        public BBParameter<string>  saveLeftText;

        protected override string info
        {
            get { return "Get Club League Remaining Time"; }
        }

        protected override void OnExecute()
        {
            DateTime targetDate = TimeUtils.ParseTimestampToDateTime(targetTime.value);

            long currentTimestamp = TimeUtils.GetTimeStamp();
            DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);

            TimeSpan leftTimespan = targetDate - currentDate;

            saveLeftTimestamp.value = (long)leftTimespan.TotalMilliseconds;

            if(saveLeftTimestamp.value > 0)
            {
                if ((int)leftTimespan.Days > 0)
                {
                    saveLeftText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_LEFT_TIME_DAY_TEXT", leftTimespan.Days, leftTimespan.Hours, leftTimespan.Minutes);
                }
                else
                {
                    saveLeftText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_LEFT_TIME_HOUR_TEXT", leftTimespan.Hours, leftTimespan.Minutes, leftTimespan.Seconds);
                }
            }
            else
            {
                saveLeftText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_BREAK_TIME_TEXT");
            }

            EndAction();
        }
    }
}
