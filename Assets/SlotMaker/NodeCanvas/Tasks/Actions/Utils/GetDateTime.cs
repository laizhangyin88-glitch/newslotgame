using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions 
{
    public enum TimeZoneType
    {
        GetUTCTime,
        GetLocalTime
    };

    [Category("★ BagelCode/TimeUtils")]
    public class GetDateTime : ActionTask 
    {
        public TimeZoneType timeZoneType;
        [BlackboardOnly]
        public BBParameter<long> timestamp;
        public BBParameter<DateTime> saveAs;

        protected override string info 
        {
            get 
            { 
                switch (timeZoneType) 
                {
                case TimeZoneType.GetUTCTime:
                    return string.Format("Get DateTime of Time Stamp of UTC as {0}", saveAs); 
                case TimeZoneType.GetLocalTime:
                    return string.Format("Get DateTime of Time Stamp of LocalTime as {0}", saveAs); 
                default:
                    return "";
                }
            }
        }

        protected override void OnExecute() 
        {
            switch (timeZoneType) 
            {
            case TimeZoneType.GetUTCTime:
                saveAs.value = MetaSystem.TimeStampToUTCDateTime(timestamp.value);
                break;
            case TimeZoneType.GetLocalTime:
                saveAs.value = MetaSystem.TimeStampToLocalDateTime(timestamp.value);
                break;
            }

            EndAction();
        }
    }
}

