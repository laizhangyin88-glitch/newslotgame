using System.Collections;
using NodeCanvas.Framework;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataMaintenance : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Maintenance Scene";
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            string message = info.GetValue<string>("message");
            SetCenterTextElement("TEXT_NORMAL", message);

            bool isHighlight = info.GetValue<bool>("isHighlighted");
            if (isHighlight)
            {
                anim.SetInteger("LayoutIndex", 7);
                yield return new WaitForSeconds(1f);
                anim.SetBool("IsActive", true);
            }
            else ActiveAnimator();

            // Wait | Skip
            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
            yield return new WaitUntilTrigger(timerTrigger, onSkipTrigger);

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }
    }
}
