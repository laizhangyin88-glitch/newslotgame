using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Meta/Shop")]
    public class UpdateShopDailyBoost : ActionTask<Blackboard> 
    {
        public BBParameter<List<Blackboard>> saveAsProductList;
        private bool isInit = false;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement itemCollectWaitTextElement;
        private ContextElement itemCollectTextElement;
        private ContextElement leftDateTextElement;
        private ContextElement remainingTimerElement;

        private ContextElement buttonElement;

        private RemainingTimerController timerController;

        protected override void OnExecute()
        {
            InitProperty();
            UpdateValues();

            EndAction();
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootAnimator = agent.gameObject.GetComponent<Animator>();

            rootElement = agent.gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            itemCollectWaitTextElement = ContextUtils.FindElement(rootElement, "Text Wait", ContextSearchingType.ChildrenSearch);
            itemCollectTextElement = ContextUtils.FindElement(rootElement, "Text Collect", ContextSearchingType.ChildrenSearch);
            leftDateTextElement = ContextUtils.FindElement(rootElement, "Text Date", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(rootElement, "Remaining Timer Text", ContextSearchingType.ChildrenSearch);;
            buttonElement = ContextUtils.FindElement(rootElement, "Collect", ContextSearchingType.ChildrenSearch);;

            timerController = agent.gameObject.AddComponent<RemainingTimerController>();

            isInit = true;
        }

        private void UpdateValues()
        {
            timerController.StopTimer();

            BlackboardQueryUtils.UpdateDailyBoostState();
            var dailyBoostBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "dailyBoost");

            bool isTerminated = true;
            
            var dailyBoostShopBB = BlackboardQueryUtils.GetShopBB(ShopType.DAILY_BOOST);
            var productGroupList = dailyBoostShopBB.GetValue<List<Blackboard>>("productGroupList");
            saveAsProductList.value = BlackboardQueryUtils.GetProductList(productGroupList[0]);

            if(dailyBoostBB != null && dailyBoostBB.value != null)
            {
                isTerminated = dailyBoostBB.value.GetValue<bool>("isTerminated");

                if(isTerminated)
                {
                    UpdateBuy();
                }
                else
                {
                    bool isCollectable = dailyBoostBB.value.GetValue<bool>("isCollectable");
                    if(isCollectable)
                    {
                        UpdateCollectable(dailyBoostBB.value);
                    }
                    else
                    {
                        UpdateCollected(dailyBoostBB.value);
                    }
                }
            }
            else
            {
                UpdateBuy();
            }
        }

        private void UpdateBuy()
        {
            var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, "product");
            productBB.value = saveAsProductList.value[0];

            var price = productBB.value.GetValue<double>("price");
            var infoBB = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.DAILY_BOOST);
            var baseCoins = infoBB.GetValue<long>("baseCreditPerDay");
            var baseGem = infoBB.GetValue<long>("baseGemPerDay");
            var rp = infoBB.GetValue<long>("rp");
            var totalDayCount = infoBB.GetValue<int>("totalDayCount");

            int tier = TierUtils.GetMeTier();
            long totalCoins = TierUtils.GetTierFractionCoin(baseCoins, tier);
            long totalGem = TierUtils.GetTierFractionCoin(baseGem, tier);
            totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "dailyBoost");

            MetaContextElementUtils.SetText(leftDateTextElement, totalDayCount.ToString());

            if (baseCoins != 0 && baseGem != 0)
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_ITEM_COIN_GEM_TEXT", totalCoins, totalGem, totalDayCount));
            }
            else if (baseGem != 0)
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_ITEM_GEM_TEXT", totalGem, totalDayCount));
            }
            else
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_ITEM_TEXT", totalCoins, totalDayCount));
            }

            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_BUY_BUTTON", price));
            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnBuyDailyBoost",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            rootAnimator.SetBool("IsDailyBoost", false);
            rootAnimator.SetBool("IsWait", false);
        }

        private void UpdateCollectable(Blackboard dailyBoostBB)
        {
            long earnCoins = dailyBoostBB.GetValue<long>("credit");
            long earnGem = dailyBoostBB.GetValue<long>("gem");
            int tier = TierUtils.GetMeTier();

            long totalCoins = TierUtils.GetTierFractionCoin(earnCoins, tier);
            long totalGem = TierUtils.GetTierFractionCoin(earnGem, tier);
            totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "dailyBoost");

            if (earnGem != 0 && earnCoins != 0)
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_COLLECTABLE_COIN_GEM_TEXT", totalCoins, totalGem));
            }
            else if (earnGem != 0)
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_COLLECTABLE_GEM_TEXT", totalGem));
            }
            else
            {
                MetaContextElementUtils.SetText(itemCollectTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_COLLECTABLE_TEXT", totalCoins));
            }

            int leftCollectCount  = dailyBoostBB.GetValue<int>("leftCollectDayCount");
            MetaContextElementUtils.SetText(leftDateTextElement, leftCollectCount.ToString());
            
            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_COLLECTABLE_BUTTON"));
            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnCollectDailyBoost",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            rootAnimator.SetBool("IsDailyBoost", true);
            rootAnimator.SetBool("IsWait", false);
        }

        private void UpdateCollected(Blackboard dailyBoostBB)
        {
            long earnCoins = dailyBoostBB.GetValue<long>("credit");
            long earnGem = dailyBoostBB.GetValue<long>("gem");
            int tier = TierUtils.GetMeTier();

            long totalCoins = TierUtils.GetTierFractionCoin(earnCoins, tier);
            long totalGem = TierUtils.GetTierFractionCoin(earnGem, tier);
            totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "dailyBoost");

            if (earnGem != 0 && earnCoins != 0)
            {
                MetaContextElementUtils.SetText(itemCollectWaitTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_NOT_COLLECTABLE_COIN_GEM_TEXT", totalCoins, totalGem));
            }
            else if (earnGem != 0)
            {
                MetaContextElementUtils.SetText(itemCollectWaitTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_NOT_COLLECTABLE_GEM_TEXT", totalGem));
            }
            else
            {
                MetaContextElementUtils.SetText(itemCollectWaitTextElement, StringTableUtils.GetString(tableType, "SHOP_DAILYBOOST_NOT_COLLECTABLE_TEXT", totalCoins));
            }

            int leftCollectCount  = dailyBoostBB.GetValue<int>("leftCollectDayCount");
            MetaContextElementUtils.SetText(leftDateTextElement, leftCollectCount.ToString());

            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnNotCollectable",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            rootAnimator.SetBool("IsDailyBoost", true);
            rootAnimator.SetBool("IsWait", true);

            // Start Timer. 
            long nextCollectTime = dailyBoostBB.GetValue<long>("nextCollectTime");
            timerController.Init(remainingTimerElement, "TIME_FORMAT_HHMMSS", "SHOP_DAILYBOOST_NOT_COLLECTABLE_BUTTON", "", "Ended", false, TimerCallback);
            timerController.StartTimer(nextCollectTime, 0);
        }

        private void TimerCallback()
        {
            UpdateValues();
        }
    }

}

