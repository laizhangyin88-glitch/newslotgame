using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils")]
    public class GetRemainningTime : ActionTask<Blackboard> // todo fix typo
    {
        public BBParameter<long>    targetTime;
        public BBParameter<long>    warningTime;
        
        public BBParameter<string>  key;
        public BBParameter<string>  outputFormatKey;
        public BBParameter<string>  warningFormatKey;
        public StringTable.StringTableType tableType;

        public BBParameter<bool>    useCommonTimer;

        [BlackboardOnly]
        public BBParameter<bool>    saveWarnning;

        [BlackboardOnly]
        public BBParameter<long>    saveAs;

        [BlackboardOnly]
        public BBParameter<string>  saveStringAs;

        public const string TIME_FORMAT_COMMON_TIME_LEFT = "TIME_FORMAT_COMMON_TIME_LEFT";

        protected override string info
        {
            get { return "Get Remaining Time"; }
        }

        protected override void OnExecute()
        {
            saveWarnning.value = false;

            DateTime targetDate = TimeUtils.ParseTimestampToDateTime(targetTime.value);

            long currentTimestamp = TimeUtils.GetTimeStamp();
            DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);

            TimeSpan leftTimespan = targetDate - currentDate;

            saveAs.value = (long)leftTimespan.TotalMilliseconds;

            if(!string.IsNullOrEmpty(key.value))
            {
                if(saveAs.value > 0)
                {
                    if(useCommonTimer.value && leftTimespan.TotalDays >= 1.0)
                    {
                        saveStringAs.value = StringTableUtils.GetString(tableType, TIME_FORMAT_COMMON_TIME_LEFT, (int)leftTimespan.TotalDays);
                    }
                    else
                    {
                        saveStringAs.value = StringTableUtils.GetString(tableType, key.value, leftTimespan.Days, leftTimespan.Hours, leftTimespan.Minutes, leftTimespan.Seconds, Math.Truncate((double)leftTimespan.TotalHours));
                    }

                    if(warningTime.value > 0 && currentTimestamp > warningTime.value && !string.IsNullOrEmpty(warningFormatKey.value))
                    {
                        saveWarnning.value = true;
                        saveStringAs.value = StringTableUtils.GetString(tableType, warningFormatKey.value, saveStringAs.value);
                    }
                    else if(outputFormatKey != null && !string.IsNullOrEmpty(outputFormatKey.value))
                    {
                        saveStringAs.value = StringTableUtils.GetString(tableType, outputFormatKey.value, saveStringAs.value);
                    }
                }
                else
                {
                    saveStringAs.value = StringTableUtils.GetString(tableType, key.value, 0, 0, 0, 0, 0);
                }
            }
            else
            {
                saveStringAs.value = "";
            }

            EndAction();
        }
    }
}
