using UnityEngine;
using SlotMaker;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsPopupFinderReceivedController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isPurchaseResult;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetTrigger("Active");

            isPurchaseResult = bb.GetValue<bool>("isPurchaseResult");

            // Title
            string titleKey = isPurchaseResult ?
                "HIDDEN_OBJECTS_POPUP_RECEIVED_PURCHASE_TITLE" :
                "HIDDEN_OBJECTS_POPUP_RECEIVED_CLUB_SHARE_TITLE";
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", titleKey, FULL);

            // Text
            MetaContextElementUtils.SimpleSetActive(root, "Item/Text", !isPurchaseResult, FULL);
            if (!isPurchaseResult)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Item/Text", "HIDDEN_OBJECTS_POPUP_RECEIVED_CLUB_SHARE_TEXT", FULL);
            }

            // Make Finder Image
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Image Reward Received Finder";
            Transform parent = ContextUtils.FindElement(root, "Item/Image Area", FULL).transform;
            var finderObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            var finderElement = finderObj.GetComponent<ContextElement>();
            finderElement.UpdateContext(true);

            // Make Badge
            asset = "Badge Big";
            parent = ContextUtils.FindElement(finderElement, "Badge Area", CHILDREN).transform;
            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            StartCoroutine(SetBadgeCoroutine(badgeObj));

            // Collect
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT", FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Collect", Close);
        }

        private IEnumerator SetBadgeCoroutine(GameObject badgeObj)
        {
            var badgeAnimElement = badgeObj.GetComponent<ContextAnimator>();

            yield return new WaitUntil(() => badgeAnimElement.isActiveAndEnabled);

            int earnFinder = bb.GetValue<int>("earnFinder");
            badgeAnimElement.UpdateContext(false);
            MetaContextElementUtils.SetIntProperty(badgeAnimElement, earnFinder);
            MetaContextElementUtils.SimpleSetText(badgeAnimElement, "Text", earnFinder.ToString());
        }

        private void Close()
        {
            if (!isPurchaseResult)
            {
                PlayEarnFinderEffect();
            }
            else
            {
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_COLLECT_FINDER).Play();
            }

            EventSender.SendCalleeCallback(gameObject);

            anim.SetTrigger("Close");
            PopupManager.Instance.Close(gameObject);
        }

        private void PlayEarnFinderEffect()
        {
            string bundle = HiddenObjects.Defines.COMMON_BUNDLE;
            string asset = "Hidden Objects In Game Finder";

            var mainSceneElement = HiddenObjects.Utils.MainScene.GetComponent<ContextElement>();
            var finderGaugeElement = ContextUtils.FindElement(mainSceneElement, "Finder Gauge", CHILDREN);
            Transform parent = finderGaugeElement.transform;

            GameObject effectGO = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            var itemElement = ContextUtils.FindElement(root, "Item", CHILDREN);
            Vector3 from = itemElement.transform.position;
            Vector3 to = parent.position;

            GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_FINDER_OBTAIN1).Play();
            
            var caller = bb.GetValue<GameObject>("caller");
            var mainController = caller.GetComponent<HiddenObjectsMainController>();
            float EFFECT_MOVEMENT_TIME = 1.2f;
            AsyncActionUtils.ApplyMovement(mainController, effectGO.transform, from, to, EFFECT_MOVEMENT_TIME, TweenUtils.VectorTweenCollectMove,
                0f, () => { if (!(effectGO is null)) Destroy(effectGO); });
        }
    }
}
