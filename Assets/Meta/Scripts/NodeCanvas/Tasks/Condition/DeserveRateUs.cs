using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Conditions{

    [Category("★ BagelCode")]
    [Description("Checks if you deserve for receiving reward for rating us.")]
    public class DeserveRateUs : ConditionTask {

        protected override string info{
            get {return "deserve for rating us reward";}
        }

        protected override bool OnCheck()
        {
            var rateUsEnabled = BlackboardUtils.FindVariable<bool>(null, "/rateUsEnabled");

            if(rateUsEnabled != null)
            {
                if(rateUsEnabled.value == true)
                {
                    int level = BlackboardUtils.FindVariable<int>(null, "/me/level").value;
                    int lastRatedVersion = BlackboardUtils.FindVariable<int>(null, "/me/lastRatedClientNumberVersion").value;
                    int clientVersion = ApplicationSettings.GetClientVersionNumber();

                    if(level >= 10 && lastRatedVersion < clientVersion)
                    {
                        rateUsEnabled.value = false;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
