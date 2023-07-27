using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

//in Main_FSM > MAIN Check Bonus_BT > Main_IOS_IDFA_BT

namespace BagelCode.Tasks.Conditions.IDFA
{
    [Category("★ BagelCode/NativeHelper/IDFA")]
    public class CheckIOSIDFAConsentResult : ConditionTask<Blackboard>
    {
        protected override bool OnCheck()
        {
            var consentStatusString = BlackboardUtils.FindValue<string>(MainBlackboard.Get(), IDFAHelper.IDFA_CONSENT_RESULT);
            return consentStatusString == "ATTrackingManagerAuthorizationStatusAuthorized";
        }
    }
}