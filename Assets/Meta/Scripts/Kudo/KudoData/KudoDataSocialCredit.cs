using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataSocialCredit : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Social Credit Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Accept Button Text
            SetButtonTextReceiveRp();
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Active Animator
            ActiveAnimator();

            // Send Bi
            string kudoType = "kudo_social_credit";
            SendBiKudo(info, "trigger", kudoType);

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Set User Name Credit
            string userName = info.GetValue<string>("name");
            long earnCredit = info.GetValue<long>("earnCredit");
            SetUserNameText("FEED_RECEIVE_SOCIAL_PURCHASE_COIN_TEXT", userName, FreebieLevelUtils.GetLevelMultiplierNumeratorValue(earnCredit, FreebieLevelUtils.FreebieType.FRIENDS_DEAL_BONUS));

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.SOCIAL_CREDIT));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
