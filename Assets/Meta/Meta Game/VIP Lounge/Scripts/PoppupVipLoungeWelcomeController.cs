using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode.VipLounge
{
    public class PoppupVipLoungeWelcomeController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private string contextId = "";

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext();

            // Badge
            int badgeCount = VipLounge.Utils.BadgeCount;
            if(badgeCount >= 1)
            {
                MetaContextElementUtils.SimpleSetActive(root, "Item 01/Active Icon", true, ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Bottom Info", "VIP_LOUNGE_WELCOME_BOTTOM_INFO", ContextSearchingType.ChildrenSearch,
                    VipLounge.Utils.MaxLoungePoint, BlackboardQueryUtils.GetTimeStampToTimeString(VipLounge.Utils.LoungeOpenTimeMillisec).ToUpper());
            }
            if(badgeCount >= 2)
            {
                MetaContextElementUtils.SimpleSetActive(root, "Item 02/Active Icon", true, ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SimpleSetActive(root, "Text Bottom Info", false);
            }

            // Text
            string expireDate = VipLounge.Utils.GetExpireDate().ToUpper();
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text 01", "VIP_LOUNGE_WELCOME_TEXT_1", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text 02", "VIP_LOUNGE_WELCOME_TEXT_2", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Expires", "VIP_LOUNGE_WELCOME_EXPIRE_DATE", ContextSearchingType.ChildrenSearch, expireDate);

            // Check
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Check/Text", "VIP_LOUNGE_WELCOME_CHECK", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Check", Check);

            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", Close);
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), Close);
            // AE
            BIClientVipLoungePopup();
        }

        private void Check()
        {
            StartCoroutine(CheckCoroutine());
        }

        private IEnumerator CheckCoroutine()
        {
            // currentOrientation Check
            Orientation currentOrientation = BlackboardQueryUtils.GetOrientation();
            if (currentOrientation != Orientation.LANDSCAPE)
            {
                MetaGameUtils.SetOrientation(Orientation.LANDSCAPE, gameObject);

                var callbackTrigger = new EventTrigger(gameObject, "OnFinishedChangeOrientation");
                yield return new WaitUntilTrigger(callbackTrigger);
            }
            // Open Vip Loading
            string contextId = GetContextId();

            // Send BI
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "ui_click");

            string bundle = VipLounge.Defines.COMMON_BUNDLE;
            string asset = "VIP Epic Lounge Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            loadingBB.AddVariable("_biContextID", contextId);
            loadingBB.AddVariable("isEnter", true);
            loadingBB.AddVariable("isMetaInGame", false);
            loadingBB.AddVariable("enter_type", "lounge_open_popup");
            if (currentOrientation != Orientation.LANDSCAPE)
                loadingBB.AddVariable("prevOrientation", currentOrientation);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_VIP_LOUNGE_WELCOME_JOIN);
            Close();
        }

        private void Close()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            anim.SetTrigger("Close");

            EventSender.SendCalleeCallback(gameObject);

            MetaPopupUtils.ClosePopup(gameObject);
        }

        private string GetContextId()
        {
            if (string.IsNullOrEmpty(contextId))
                contextId = BiEventUtils.GenerateContextID();
            return contextId;
        }

        private void BIClientVipLoungePopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["popup_name"] = "vip_lounge_open";
            customData["context_id"] = GetContextId();

            Analytics.CustomEvent("client_vip_lounge_popup", customData);
        }
    }
}
