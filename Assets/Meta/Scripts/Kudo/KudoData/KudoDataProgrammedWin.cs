

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataProgrammedWin : KudoDataLike
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Jackpot Like Scene";
        }

        public override IEnumerator DisplayKudoCoroutine()
        {
            yield return controller.StartCoroutine(UpdateKudoData(null));
            OnDisappearKudo(true);
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            if (feedInfoList.Count == 0) yield break;

            yield return new WaitForSeconds(4f);

            // Active Animator
            ActiveAnimator();

            // Send Bi
            SendBiKudo("trigger", "kudo_receive");

            // Deactive Profile Area
            SetProfileActive(3, false);
            MetaContextElementUtils.SimpleSetActive(root, "Button Close", false);

            var timer = 0f;

            var feed = feedInfoList.FirstOrDefault();
            var likeCount = feed.GetValue<int>("likeCount");
            var winType = feed.GetValue<WinType>("winType");
            var profileUrlList = feed.GetValue<List<string>>("profileUrlList");

            UpdateProfile(profileUrlList);

            // Update Kudo
            while (timer <= 5f)
            {
                UpdateLikeKudo((int)Mathf.Lerp(0, likeCount, timer / 5f), winType);

                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            UpdateLikeKudo(likeCount, winType);
            MetaContextElementUtils.SimpleSetActive(root, "Button Close", true);

            var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
            var onSkipTrigger = new EventTrigger(controller, ON_SKIP);

            while (true)
            {
                // Wait | Skip
                timerTrigger.Update();
                if (timerTrigger.IsTrigger || onSkipTrigger.IsTrigger) break;

                yield return new WaitForEndOfFrame();
            }

            // Disappear
            yield return controller.StartCoroutine(DisappearCoroutine());
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            likeTextKey = "FEED_RECEIVE_PROGRAMMED_{0}";
        }

        private void UpdateProfile(List<string> profileUrlList)
        {
            for (int k = 0; k < profileUrlList.Count; k++)
            {
                controller.StartCoroutine(SetFakeProfileCoroutine(k, profileUrlList[k]));
            }
        }

        private void UpdateLikeKudo(int likeCount, WinType winType)
        {
            SetCenterTextElement(string.Format(likeTextKey, winType), likeCount - 3);
        }
    }
}