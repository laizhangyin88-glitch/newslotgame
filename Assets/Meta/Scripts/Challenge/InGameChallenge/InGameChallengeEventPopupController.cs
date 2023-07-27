using System.Collections;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class InGameChallengeEventPopupController : MonoBehaviour
    {
        public const float DISPLAY_TIME = 2f;

        private ContextElement root;
        private Animator anim;

        private ContextElement remainingTimerElement;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            remainingTimerElement = ContextUtils.FindElement(root, "Remaining Timer", FULL);

            isInit = true;
        }

        public IEnumerator DisplayPopupCoroutine(EventInfo eventInfo)
        {
            if(eventInfo == null)
            {
                Destroy(gameObject);
                yield break;
            }

            InitProperty();

            bool isEventChallenge = ChallengeUtils.IsEventChallengePassiveEvent(eventInfo);

            remainingTimerElement.gameObject.SetActive(isEventChallenge);

            if(isEventChallenge)
            {
                // Wait Content Initialize
                yield return new WaitUntil(() => BlackboardUtils.FindVariable<bool>("./initializedContent")?.value ?? false);

                anim.SetBool("IsActive", true);

                // Set Timer
                long endTimestamp = eventInfo.endTimestamp;
                MetaContextElementUtils.SetCommonRemainingTimer(remainingTimerElement, endTimestamp, 0,
                    "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, null);

                yield return new WaitForSeconds(DISPLAY_TIME);

                anim.SetBool("IsActive", false);

                var onEndAnim = new EventTrigger(gameObject, MetaEventDefine.ON_META_UI_EVENT, "OnEndEventChallengePopupAnim");
                yield return new WaitUntilTrigger(onEndAnim);
            }
        }
    }
}
