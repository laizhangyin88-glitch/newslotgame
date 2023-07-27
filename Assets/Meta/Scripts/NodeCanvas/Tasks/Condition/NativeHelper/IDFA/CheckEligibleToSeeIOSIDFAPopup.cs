using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
///using BagelCode.NativeBridge.IOSIDFAManager;
using BagelCode.Utils;
using SlotMaker;

//in Main_FSM > MAIN Check Bonus_BT > Main_IOS_IDFA_BT

namespace BagelCode.Tasks.Conditions.IDFA
{
    [Category("★ BagelCode/NativeHelper/IDFA")]
    public class CheckEligibleToSeeIOSIDFAPopup : ConditionTask<Blackboard>
    {
        protected override bool OnCheck()
        {
            return IDFAHelper.Instance.IsEligibleToSeeIDFAConsentPopup() && !IDFAHelper.Instance.HasSeenIDFAConsentPopup();
        }
    }
}
