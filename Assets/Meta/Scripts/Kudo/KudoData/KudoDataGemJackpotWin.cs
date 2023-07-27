using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataGemJackpotWin : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Gold Tower Jackpot Win Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Button Text
            SetButtonTextReceiveRp();

            // Make Wheel Icon
            // MakeWheelIcon(false);

            // Set Jackpot Text
            // SetJackpotText("FEED_TEXT_JACKPOT");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Play Trigger Sound
            GSManager.Instance.GetHandler("UI_JackpotTrigger").Play();

            // Send Bi
            string kudoType = "kudo_jackpot";
            SendBiKudo(info, "trigger", kudoType);

            // Active Anim
            ActiveAnimator();

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Set Flexible Text
            long winCredit = info.GetValue<long>("winCredit");

            SetFlexibleText("FEED_GEM_JACKPOT_TEXT", winCredit);

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if(onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.META_JACKPOT_WIN));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
