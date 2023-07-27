using System.Collections;
using NodeCanvas.Framework;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public abstract class KudoDataLike : KudoData
    {
        protected string likeTextKey;

        public override IEnumerator DisplayKudoCoroutine()
        {
            yield return controller.StartCoroutine(UpdateKudoData(null));
            OnDisappearKudo(true);
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            if (feedInfoList.Count == 0) yield break;

            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_receive");

            // Deactive Profile Area
            SetProfileActive(3, false);

            int progress = 0;

            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);

            // Update Kudo
            while (feedInfoList.Count > 0)
            {
                int likeCount = feedInfoList.Count;
                if (likeCount > progress)
                {
                    UpdateLikeKudo(++progress);
                    timerTrigger.Reset();
                }

                // Wait | Skip
                timerTrigger.Update();
                if (timerTrigger.IsTrigger || onSkipTrigger.IsTrigger) break;

                yield return new WaitForEndOfFrame();
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }

        private void UpdateLikeKudo(int likeCount)
        {
            int i = likeCount - 1;
            if (likeCount < 4)
            {
                // Show Profile
                controller.StartCoroutine(SetProfileCoroutine(i, feedInfoList[i]));

                SetCenterTextElement(string.Format(likeTextKey, i));
            }
            else if(likeCount == 4)
            {
                SetCenterTextElement(string.Format(likeTextKey, i));
            }
            else if(likeCount > 4)
            {
                SetCenterTextElement(string.Format(likeTextKey, 4), likeCount - 3);
            }
        }
    }
}
