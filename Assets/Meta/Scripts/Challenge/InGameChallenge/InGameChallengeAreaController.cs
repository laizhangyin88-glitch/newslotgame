using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;

namespace BagelCode
{
    public class InGameChallengeAreaController : MonoBehaviour
    {
        public void OnEnableEventChallenge()
        {
            EventSender.SendEvent(gameObject, ChallengeEventManager.ON_ENABLE_EVENT_CHALLENGE);
        }

        public IEnumerator MakeSceneCoroutine(bool isClub, Blackboard completeInfo)
        {
            yield return new WaitUntil(() => !BlackboardQueryUtils.IsHideUI());

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = isClub ? "In Game Challenge Club Layer Scene" : "In Game Challenge Layer Scene";

            GameObject layerObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, transform,
                (GameObject obj) => layerObj = obj));

            Blackboard layerBB = layerObj.GetComponent<Blackboard>();
            layerBB.AddVariable("_completeInfo", completeInfo);

            MetaObjectUtils.SetCalleeCaller(layerObj, gameObject);
            layerObj.SetActive(true);

            // Wait Complete Scene End
            var onCalleeCallbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(onCalleeCallbackTrigger);

            // Complete Effect
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                new EventData<Blackboard>(ChallengeEventManager.ON_COMPLETE_CHALLENGE_MISSION_EFFECT, completeInfo));
        }

        public IEnumerator OnEnablePassiveEventCoroutine()
        {
            var eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            if (!ChallengeUtils.IsEventChallengePassiveEvent(eventInfo)) yield break;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "In Game Challenge Event Popup";

            GameObject notiPopup = null;
            yield return StartCoroutine(MetaObjectUtils.MakePrefabCoroutine(bundle, asset, transform,
                (GameObject obj) => notiPopup = obj));

            MetaObjectUtils.SetCalleeCaller(notiPopup, gameObject);

            var controller = notiPopup.GetComponent<InGameChallengeEventPopupController>();
            yield return StartCoroutine(controller.DisplayPopupCoroutine(eventInfo));

            Destroy(notiPopup);
        }
    }
}
