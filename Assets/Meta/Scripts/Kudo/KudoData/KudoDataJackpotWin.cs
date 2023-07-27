using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataJackpotWin : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Jackpot Win Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Active Image Area
            ActiveJackpotImageArea();

            // Set Button Text
            SetButtonTextReceiveRp();
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Play Trigger Sound
            GSManager.Instance.GetHandler("UI_JackpotTrigger").Play();

            // Send Bi
            string kudoType = "kudo_jackpot";
            SendBiKudo(info, "trigger", kudoType);

            int gameId = info.GetValue<int>("gameId");
            var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);

            // Active Anim
            ActiveAnimator();

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Make Slot Thumbnail
            MakeSlotThumbnailIcon(gameInfo, true);

            // Set Flexible Text
            long winCredit = info.GetValue<long>("winCredit");
            SetFlexibleText("FEED_GAME_JACKPOT_TEXT", winCredit);

            // Set Jackpot Text
            var jackpotKudoType = info.GetValue<JackpotKudoType>("jackpotKudoType");
            if(jackpotKudoType != JackpotKudoType.UNKNOWN)
            {
                SetJackpotText(string.Format("FEED_TEXT_{0}_JACKPOT", jackpotKudoType.ToString()));
            }
            else
            {
                SetJackpotText("FEED_TEXT_JACKPOT");
            }

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.JACKPOT_WIN));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
