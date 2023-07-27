using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class DailyWheelPurchaseItemCellController : MonoBehaviour
    {
        private Blackboard rootBB;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement buyButtonElement;
        private ContextElement buyButtonTextElement;
        private ContextElement freeButtonElement;
        private ContextElement spinsTextElement;
        private ContextElement rpTextElement;

        private Blackboard productBB;
        private bool fromLogin;
        public int cellIndex;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string FREE_WHEEL_LAST_SHOW_TIMESTAMP = "FREE_WHEEL_LAST_SHOW_TIMESTAMP";

        private bool isInit = false;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            // Button Buy += OnBuyItem
            buyButtonElement = ContextUtils.FindElement(rootElement, "Button Buy", ContextSearchingType.ChildrenSearch);
            buyButtonTextElement = ContextUtils.FindElement(buyButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            spinsTextElement = ContextUtils.FindElement(rootElement, "Text Spin Count", ContextSearchingType.ChildrenSearch);
            rpTextElement = ContextUtils.FindElement(rootElement, "Text VIP Point", ContextSearchingType.ChildrenSearch);
            isInit = true;
        }

        public void UpdateValues()
        {
            InitProperty();

            cellIndex = rootBB.GetValue<int>("cellIndex");
            fromLogin = rootBB.GetValue<bool>("fromLogin");
            var productList = rootBB.GetValue<List<Blackboard>>("productList");

            productBB = productList[0];

            if (cellIndex > 2) cellIndex = 2;
            rootAnimator.SetInteger("Item", cellIndex);

            var infoBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.DAILY_BONUS_WHEEL);
            long earnRP    = infoBB.GetValue<long>("rp");
            int spinCount = infoBB.GetValue<int>("spinCount");
            float itemPrice = System.Convert.ToSingle(productBB.GetValue<double>("price"));
            float origItemPrice = System.Convert.ToSingle(productBB.GetValue<double>("originalPrice"));

            MetaContextElementUtils.SetTextGlobal(spinsTextElement, "POPUP_DAILY_SPINS_COUNT", spinCount);
            MetaContextElementUtils.SetTextGlobal(rpTextElement, "POPUP_DAILY_SPIN_VIP", earnRP);
            MetaContextElementUtils.SetTextGlobal(buyButtonTextElement, "SHOP_BUY_BUTTON", itemPrice);

            // from login, price 2 < = video ads.
            // Check Cool Time 24H
            if( itemPrice < 1f && fromLogin && ShowADSEnabled() )
            {
                if(freeButtonElement == null)
                {
                    // Make  / <sprite name=AD>FREE
                    var freeButtonObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Yellow", buyButtonElement.transform.parent);
                    freeButtonElement = freeButtonObj.GetComponent<ContextElement>();
                    freeButtonElement.UpdateContext(false);

                    var freeButtonTextElement = ContextUtils.FindElement(freeButtonElement, "Text", ContextSearchingType.ChildrenSearch);
                    MetaContextElementUtils.SetTextGlobal(freeButtonTextElement, "POPUP_DAILY_SPIN_FREE_TEXT", itemPrice);
                }

                MetaContextElementUtils.SetClickable(
                    freeButtonElement,
                    "OnPlayADS",
                    rootElement,
                    null
                );

                buyButtonElement.gameObject.SetActive(false);
                freeButtonElement.gameObject.SetActive(true);
            }
            else
            {
                buyButtonElement.gameObject.SetActive(true);
                if(freeButtonElement != null)
                    freeButtonElement.gameObject.SetActive(false);

                MetaContextElementUtils.SetClickable(
                    buyButtonElement,
                    "OnBuyItem",
                    rootElement,
                    null
                );
            }
        }

        private bool ShowADSEnabled()
        {
            var enabledADS = BlackboardUtils.GetOrCreateVariable<bool>(null, "/values/misc/VIDEO_ADS_ENABLED");
            if( !enabledADS.value )
                return false;

            var placement = BlackboardUtils.GetOrCreateVariable<string>(null, "/videoAdsPlacementNames/dailyBonus");
            if(placement == null || string.IsNullOrEmpty(placement.value))
                return false;

#if !UNITY_EDITOR
            if( !VideoAdsController.Instance.IsVideoAdsAvailable(placement.value ))
                return false;
#endif

            var lastTimestamp = PlayerPrefsUtils.GetOrCreateInt64(FREE_WHEEL_LAST_SHOW_TIMESTAMP, 0);
            if(lastTimestamp != 0L)
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();
                long targetTimestamp = GetCoolTimestamp(lastTimestamp);

                if (currentTimestamp < targetTimestamp)
                {
                    return false;
                }
            }

            return true;
        }

        public void SaveLastShowADS()
        {
            long currentTimestamp = TimeUtils.GetTimeStamp();
            PlayerPrefsUtils.SetInt64(FREE_WHEEL_LAST_SHOW_TIMESTAMP, currentTimestamp);
            PlayerPrefs.Save();
        }

        private long GetCoolTimestamp(long timestamp)
        {
#if DEV
            return timestamp + TimeUtils.ONE_MIN_MS;
#else
            return timestamp + TimeUtils.ONE_DAY_MS;
#endif
        }
    }
}
