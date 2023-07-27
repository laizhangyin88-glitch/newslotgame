using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;

namespace NodeCanvas.Tasks.Conditions{

    [Category("★ BagelCode")]
    public class CheckWelcomeFeedCooltime : ConditionTask
    {
        private const string LAST_WELCOME_FEED_VIEW_TIME = "LAST_WELCOME_FEED_VIEW_TIME";

        protected override string info
        {
            get
            {
                return string.Format("LastViewTimeMS >= currentTime + WELCOME_KUDO_COOLTIME_MS");
            }
        }

        protected override bool OnCheck()
        {
            var coolTimeMS = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "values/misc/WELCOME_KUDO_COOLTIME_MS");

            if(coolTimeMS.value > 0)
            {
                long currentTimeMS = TimeUtils.GetTimeStamp();
                long lastViewTimeMS = PlayerPrefsUtils.GetOrCreateInt64(LAST_WELCOME_FEED_VIEW_TIME);

                if(currentTimeMS < lastViewTimeMS + coolTimeMS.value)
                    return false;

                PlayerPrefsUtils.SetInt64(LAST_WELCOME_FEED_VIEW_TIME, currentTimeMS);
            }

            return true;
        }
    }
}
