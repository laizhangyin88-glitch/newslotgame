using System.Collections;
using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode
{
    public class InGameChallengeCompleteAreaController : EventMonoBehaviour
    {
        public IEnumerator OpenCompleteChallengePopupCoroutine(Blackboard completeInfo)
        {
            yield return StartCoroutine(MakeCompletePopupObj(completeInfo, true));

            // Refresh CanvasGroup Interactable
            UnityEngine.CanvasGroup canvasGroup = GetComponent<UnityEngine.CanvasGroup>();
            bool interactableValue = canvasGroup.interactable;
            canvasGroup.interactable = !interactableValue;
            canvasGroup.interactable = interactableValue;

            // Close Popup
            var onCalleeCallbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(onCalleeCallbackTrigger);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.ON_END_COMPLETE_CHALLENGE_POPUP);
        }

        public IEnumerator OpenCompleteClubChallengePopupCoroutine(Blackboard completeInfo)
        {
            yield return StartCoroutine(MakeCompletePopupObj(completeInfo, false));

            // Close Popup
            var onCalleeCallbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(onCalleeCallbackTrigger);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.ON_END_COMPLETE_CHALLENGE_POPUP);
        }

        private IEnumerator MakeCompletePopupObj(Blackboard completeInfo, bool isPersonal)
        {
            GameObject completeObj = null;
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isPersonal ? "In Game Challenge Complete Popup Scene" :
                "In Game Challenge Club Complete Popup Scene";
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, transform,
                (GameObject obj) => completeObj = obj));

            Blackboard competeObjBB = completeObj.GetComponent<Blackboard>();
            competeObjBB.AddVariable("_completeInfo", completeInfo);
            competeObjBB.AddVariable("caller", gameObject);
            completeObj.SetActive(true);
        }
    }
}
