using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Common")]
    public class UpdateMetaGameRemainingTimer : ActionTask<Blackboard>
    {
        public BBParameter<ContextElement> timerAreaElement;
        public BBParameter<ContextElement> timerElement;

        public BBParameter<long>    targetTime;

        public BBParameter<bool>    saveAsIsShow;

        protected override string info
        {
            get { return "Update Meta Game Remaining Time"; }
        }

        protected override void OnExecute()
        {
            saveAsIsShow.value = MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement.value, timerElement.value, targetTime.value);
            // if(timerAreaElement.value != null && timerElement.value != null)
            // {
            //     DateTime targetDate = TimeUtils.ParseTimestampToDateTime(targetTime.value);

            //     long currentTimestamp = TimeUtils.GetTimeStamp();
            //     DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);

            //     TimeSpan leftTimespan = targetDate - currentDate;

            //     if((int)leftTimespan.TotalDays > 99)
            //     {
            //         timerAreaElement.value.gameObject.SetActive(false);
            //         saveAsIsShow.value = false;
            //     }
            //     else
            //     {
            //         timerAreaElement.value.gameObject.SetActive(true);
            //         saveAsIsShow.value = true;

            //         MetaContextElementUtils.SetCommonRemainingTimer(
            //             timerElement.value,
            //             targetTime.value,
            //             0,
            //             "TIME_FORMAT_HHMMSS_TOTALHOUR",
            //             "",
            //             "",
            //             StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_ENDED"),
            //             true,
            //             null
            //         );
            //     }
            // }

            EndAction();
        }
    }
}
