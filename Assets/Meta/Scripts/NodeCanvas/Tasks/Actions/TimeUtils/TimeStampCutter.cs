using UnityEngine;
using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils")]
    public class TimeStampCutter : ActionTask
    {
        public BBParameter<long> timestamp;

        public BBParameter<bool> woHours;
        public BBParameter<bool> woMinites;
        public BBParameter<bool> woSeconds;
        public BBParameter<bool> woMilliSeconds;

        [BlackboardOnly]
        public BBParameter<long> saveAs;

        protected override string info
        {
            get
            {
                return string.Format("Time Cutter {0}", timestamp);
            }
        }

        protected override void OnExecute()
        {
            DateTime time = TimeUtils.ParseTimestampToDateTime(timestamp.value);

            if(woHours.value)
                time = time.AddHours(-time.Hour);

            if(woMinites.value)
                time = time.AddMinutes(-time.Minute);

            if(woSeconds.value)
                time = time.AddSeconds(-time.Second);

            if(woMilliSeconds.value)
                time = time.AddMilliseconds(-time.Millisecond);

            saveAs.value = (long)((time - TimeUtils.Jan1St1970).TotalMilliseconds);

            // Debug.LogError(saveAs.value);

            EndAction();
        }
    }
}

