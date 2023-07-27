using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataFirstGlobalChat : KudoData
    {
        private const string LAST_WELCOME_FEED_VIEW_TIME = "LAST_WELCOME_FEED_VIEW_TIME";

        protected override string GetKudoSceneName()
        {
            return "Kudo First Global Chat Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Button Text
            SetButtonTextReceiveRp();

            // Set Flexible Text
            SetFlexibleText("FEED_FIRST_GLOBAL_CHAT_TEXT");
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Check CoolTime
            bool showKudo = true;
            var coolTimeMS = BlackboardUtils.FindValue<long>("/values/misc/WELCOME_KUDO_COOLTIME_MS");
            if (coolTimeMS > 0)
            {
                long currentTimeMS = TimeUtils.GetTimeStamp();
                long lastViewTimeMS = PlayerPrefsUtils.GetOrCreateInt64(LAST_WELCOME_FEED_VIEW_TIME);

                if (currentTimeMS < lastViewTimeMS + coolTimeMS)
                    showKudo = false;

                PlayerPrefsUtils.SetInt64(LAST_WELCOME_FEED_VIEW_TIME, currentTimeMS);
            }

            if (!showKudo) yield break;

            // Active Animator
            ActiveAnimator();

            // Send Bi
            string kudoType = "kudo_first_global_chat";
            SendBiKudo(info, "trigger", kudoType);

            // Set Profile
            yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

            // Accept | Wait | Skip
            var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

            // On Accept
            if (onAcceptTrigger.IsTrigger)
            {
                yield return controller.StartCoroutine(
                    OnAcceptCoroutine(info, kudoType, KudoLikeType.FIRST_GLOBAL_CHAT));
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
