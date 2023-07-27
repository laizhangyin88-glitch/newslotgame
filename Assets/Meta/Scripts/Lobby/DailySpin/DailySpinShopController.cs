using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class DailySpinShopController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement osaRectElement;

        private List<Blackboard> productGroupList;
        private Blackboard shopBB;
        private string shopContextId;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const string SHOP_OPENED_FROM_TYPE = "SHOP_OPENED_FROM_TYPE";
        private const string TYPE_DEFAULT = "default";

        private void Start()
        {
            InitProperty();
            OpenShop();
            StartCoroutine(WaitPurchaseSuccessCoroutine());
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Close
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", ClosePopup, true, CHILDREN);
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), ClosePopup);

            // Title Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DAILY_SPIN_SHOP_TITLE", FULL);

            osaRectElement = ContextUtils.FindElement(root, "Scroll Rect", FULL);

            isInit = true;
        }

        private void OpenShop()
        {
            productGroupList = BlackboardQueryUtils.GetShopProductGroups(ShopType.DAILY_BONUS);
            shopBB = BlackboardQueryUtils.GetShopBB(ShopType.DAILY_BONUS);

            SendBiEvent();

            int meTier = TierUtils.GetMeTier();
            int tierGroup = MetaSystem.GetTierGroup(meTier);
            anim.SetInteger("Tier", tierGroup);
            anim.SetBool("Active", true);

            StartCoroutine(CreateShopItemsCoroutine());
        }

        private IEnumerator CreateShopItemsCoroutine()
        {
            // Wait for OSA initialize
            yield return new WaitForEndOfFrame();

            var waitUntilAppearTrigger = new WaitUntilConditionTrigger(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Appear"));
            yield return new WaitUntilTrigger(waitUntilAppearTrigger);

            bool fromLogin = bb.GetVariable<bool>("fromLogin")?.value ?? false;

            var caller = bb.GetVariable<GameObject>("caller")?.value;

            var osaShopItems = osaRectElement.GetComponent<OSA_DailySpinShopItems>();
            osaShopItems.CreateItemList(caller, root, productGroupList, fromLogin, shopContextId);
        }

        private IEnumerator WaitPurchaseSuccessCoroutine()
        {
            var onDailySpinPurchaseSuccessTrigger = new EventTrigger(this, "OnDailySpinPurchaseSuccess");
            yield return new WaitUntilTrigger(onDailySpinPurchaseSuccessTrigger);
            ClosePopup();
        }

        private void ClosePopup()
        {
            EventSender.SendCalleeCallback(gameObject, "OnFinishedCallee");
            anim.SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void SendBiEvent()
        {
            ShopType shopType = ShopType.DAILY_BONUS;
            int shopId = shopBB.GetValue<int>("id");
            if(string.IsNullOrEmpty(shopContextId))
                shopContextId = BiEventUtils.GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["shop_type"] = shopType.ToString();
            customData["shop_id"] = shopId;
            customData["context_id"] = shopContextId;

            var eventInfo = PassiveEventUtils.GetPassiveEvent(shopType);
            customData["shop_event_flag"] = eventInfo == null ? false : true;

            if (PlayerPrefs.HasKey(SHOP_OPENED_FROM_TYPE))
            {
                string openType = PlayerPrefs.GetString(SHOP_OPENED_FROM_TYPE);

                if (!string.IsNullOrEmpty(openType))
                    customData["open_type"] = openType;
                else
                    customData["open_type"] = TYPE_DEFAULT;

                PlayerPrefs.DeleteKey(SHOP_OPENED_FROM_TYPE);
            }
            else
            {
                customData["open_type"] = TYPE_DEFAULT;
            }
            BiEventUtils.AppendLevelMultiplierEventData(customData, "wheel");
            Analytics.CustomEvent("client_store_opened", customData);
            AdjustManager.Instance.SendEvent("store_opened");
        }
    }
}
