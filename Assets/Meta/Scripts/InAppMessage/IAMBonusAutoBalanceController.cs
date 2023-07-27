using System.Collections.Generic;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.Contents;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode.InAppMessage
{
    public class IAMBonusAutoBalanceController : MonoBehaviour
    {
        private ContextElement ownerElement;
        private ContextElement agent;
        private ContextElement betAreaElement;
        private ContextElement betTextElement;
        private ContextElement gemTextElement;
        private ContextElement buttonElement;
        private ContextElement saleAreaElement;
        private Blackboard blackboard;

        private List<IAMUtils.BonusInfo> bonusInfoList;

        private int gameId;
        private int ticketedBonusId;
        private long winxNumerator;
        private bool closeWhenSucceeded;

        // private int bonusIndex;
        public IAMUtils.BonusInfo selectBonusInfo = null;
        public Blackboard selectProduct = null;

        private Blackboard buyProduct = null;

        private PIDButton button;

        private static string ON_CLICK_BUTTON_EVENT = "OnClickButton";

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private List<IAMBonusController> iamBonusList;

        private long targetBaseGem;
        private long targetTotalGem;
        private long targetBaseBet;
        private long targetTotalBet;

        private bool interactable;

        private EventInfo bonusEventInfo;
        private EventInfo gemEventInfo;

        // private MessageDelegates passiveDelegates;

        // private void Awake()
        // {
        //     passiveDelegates = new MessageDelegates
        //     (
        //         new Dictionary<string, MessageDispatcher.EventDelegate>
        //         {
        //             { "StartPassive", StartPassive },
        //             { "RefreshPassive", RefreshPassive }
        //         }
        //     );
        // }

        // private void OnEnable()
        // {
        //     MessageDispatcher.Register("OnPassiveEvent", passiveDelegates.Delegate);
        // }

        // private void OnDisable()
        // {
        //     MessageDispatcher.UnRegister("OnPassiveEvent", passiveDelegates.Delegate);
        // }

        public void Init(GameObject owner, Blackboard componentBB, Blackboard iamInfo, bool _interactable)
        {
            ownerElement = owner.GetComponent<ContextElement>();
            blackboard = GetComponent<Blackboard>();
            agent = GetComponent<ContextElement>();

            agent.UpdateContext();

            interactable = _interactable;

            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "componentBB", componentBB);
            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "_iamInfo", iamInfo);

            closeWhenSucceeded = componentBB.GetValue<bool>("closeWhenSucceeded");

            gameId = componentBB.GetValue<int>("gameId");
            ticketedBonusId = componentBB.GetValue<int>("ticketedBonusId");
            winxNumerator = BlackboardQueryUtils.GetWinXNumeratorFromTicketedBonusID(ticketedBonusId);

            long minGem = componentBB.GetValue<long>("minGem");
            long maxGem = componentBB.GetValue<long>("maxGem");

            bonusInfoList = IAMUtils.GetBonusInfoList(minGem, maxGem, winxNumerator, gameId);

            saleAreaElement = ContextUtils.FindElement(agent, "Sale", ContextSearchingType.ChildrenSearch);
            saleAreaElement.gameObject.SetActive(false);

            betAreaElement = ContextUtils.FindElement(agent, "Gold Type", ContextSearchingType.ChildrenSearch);
            betTextElement = ContextUtils.FindElement(betAreaElement, "Bet Text", ContextSearchingType.ChildrenSearch);
            gemTextElement  = ContextUtils.FindElement(agent, "Gem Type", ContextSearchingType.ChildrenSearch);
            buttonElement = ContextUtils.FindElement(agent, "IAM PlayNow Button", ContextSearchingType.ChildrenSearch);

            UpdateUI();
        }

        public void UpdateUI()
        {
            long betCredit = BlackboardUtils.FindVariable<long>(null, "./betCredit")?.value ?? 0L;
            bool betFound = false;

            for (int i = 0; i < bonusInfoList.Count; i++)
            {
                if (bonusInfoList[i].bet == betCredit)
                {
                    selectBonusInfo = bonusInfoList[i];
                    betFound = true;
                    break;
                }
            }

            targetBaseGem = 0L;
            targetTotalGem = 0L;
            targetBaseBet = 0L;
            targetTotalBet = 0L;

            bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);
            gemEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BAB_SHOP_EVENT_MULTIPLY);
            BlackboardUtils.SetOrCreateValue<int>(blackboard, "_gemBabShopMultiplierEventID", gemEventInfo == null ? 0 : gemEventInfo.id);

            if (!betFound && bonusInfoList.Count > 0)
            {
                if (betCredit >= bonusInfoList[bonusInfoList.Count - 1].bet)
                    selectBonusInfo = bonusInfoList[bonusInfoList.Count - 1];
                else if (betCredit <= bonusInfoList[0].bet)
                    selectBonusInfo = bonusInfoList[0];
            }

            if(selectBonusInfo != null)
            {
                long meGem = BlackboardUtils.FindVariable<long>(null, "/me/gem").value;

                targetTotalGem = targetBaseGem = selectBonusInfo.gem;

                if (bonusEventInfo != null)
                {
                    if(bonusEventInfo.type == EventInfoType.BONUS_SALE)
                    {
                        targetTotalGem = NumberUtils.GetSaleNumeratorValue(targetBaseGem, PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo));
                        targetTotalGem = IAMUtils.RemoveUnitOfDigit(targetTotalGem);
                    }
                }

                if(targetTotalGem < meGem)
                {
                    PlayNowWithGem();
                }
                else
                {
                    var productGroupList = BlackboardQueryUtils.GetShopProductGroups(ShopType.GEM_BAB);
                    if(productGroupList == null && productGroupList.Count > 0)
                    {
                        // Goto shop
                        ExceptionGoToShop();
                    }
                    else
                    {
                        int meTier = TierUtils.GetMeTier();

                        long selectGem = long.MaxValue;
                        selectProduct = null;
                        buyProduct = null;

                        for(int i=0; i<productGroupList.Count; ++i)
                        {
                            var productList = BlackboardQueryUtils.GetProductList(productGroupList[i]);
                            for(int k=0; k<productList.Count; ++k)
                            {
                                var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(productList[k], ItemType.GEM);
                                if(gemItemBB != null)
                                {
                                    long baseGem = gemItemBB.GetValue<long>("gem");
                                    long totalGem = TierUtils.GetTierFractionCoin(baseGem, meTier);
                                    totalGem = GetEventGem(totalGem, gemEventInfo);

                                    if(totalGem < selectGem && meGem + totalGem >= targetTotalGem)
                                    {
                                        selectGem = totalGem;
                                        selectProduct = productList[k];
                                    }

                                    buyProduct = productList[k];
                                }
                            }
                        }

                        if(buyProduct == null)
                        {
                            ExceptionGoToShop();
                        }
                        else
                        {
                            if(selectProduct != null)
                            {
                                PlayNowPurchaseGem();
                            }
                            else
                            {
                                selectProduct = buyProduct;
                                BuyNow();
                            }
                        }
                    }
                }
            }

            MakeBlackboardGemProduct();
            MakeBlackboardBuyBonusWithGemProduct();
        }

        private void UpdatePlayBetUI()
        {
            saleAreaElement.gameObject.SetActive(false);

            if(selectProduct != null)
            {
                var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(selectProduct, ItemType.GEM);
                // Debug.LogError(selectProduct.GetValue<double>("price"));
                // Debug.LogError(selectProduct.GetValue<int>("id"));
                long baseGem = gemItemBB.GetValue<long>("gem");
                int meTier = TierUtils.GetMeTier();
                long totalGem = TierUtils.GetTierFractionCoin(baseGem, meTier);
                totalGem = GetEventGem(totalGem, gemEventInfo);
                // Debug.LogError(totalGem);
            }
            else
            {
                // Debug.LogError("Select Product Null");
            }

            long rawBaseBet = BlackboardQueryUtils.GetRawBaseBet(gameId);
            targetTotalBet = targetBaseBet = selectBonusInfo.bet + selectBonusInfo.extraBet;

            if (bonusEventInfo != null)
            {
                if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                {
                    long numerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);
                    targetTotalBet = IAMUtils.RemoveUnitOfDigit( NumberUtils.GetMultiplierNumeratorValue(targetBaseBet, numerator) );

                    if(BlackboardQueryUtils.IsBuyABonusEventPercentText())
                    {
                        long viewAddPercent = NumberUtils.GetAdditionalPercent(numerator);
                        MetaContextElementUtils.SimpleSetText(saleAreaElement, "Sale Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BADGE_PERCENT_MORE_TEXT", viewAddPercent));
                    }
                    else
                    {
                        MetaContextElementUtils.SimpleSetText(saleAreaElement, "Sale Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BADGE_MULTIPLY_BET_TEXT", NumberUtils.GetMultiplierFromNumerator(numerator)));
                    }

                    saleAreaElement.gameObject.SetActive(true);
                }
            }

            if(targetBaseBet == targetTotalBet)
            {
                MetaContextElementUtils.SetText(betTextElement, StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_TOTAL_COIN_TEXT", targetTotalBet));
            }
            else
            {
                MetaContextElementUtils.SetText(betTextElement, StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_EVENT_TOTAL_COIN_TEXT", targetBaseBet, targetTotalBet));
            }

            // Check Bonus event passive. multiply bet.

            betAreaElement.gameObject.SetActive(true);
            gemTextElement.gameObject.SetActive(false);
        }

        private void UpdateBuyProductUI()
        {
            var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(selectProduct, ItemType.GEM);
            // Debug.LogError(selectProduct.GetValue<double>("price"));
            // Debug.LogError(selectProduct.GetValue<int>("id"));
            // Debug.LogError(gemItemBB.GetValue<long>("gem"));
            double price = selectProduct.GetValue<double>("price");
            long baseGem = gemItemBB.GetValue<long>("gem");
            int meTier = TierUtils.GetMeTier();
            long totalGem = TierUtils.GetTierFractionCoin(baseGem, meTier);

            if (gemEventInfo != null && gemEventInfo.type == EventInfoType.GEM_BAB_SHOP_EVENT_MULTIPLY)
            {
                long eventTotalGem = GetEventGem(totalGem, gemEventInfo);
                // Badge Area Active
                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM) )
                    MetaContextElementUtils.SimpleSetText(saleAreaElement, "Sale Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BADGE_PERCENT_MORE_TEXT", PassiveEventManager.Instance.GetEventInfoViewPercent(gemEventInfo)));
                else
                    MetaContextElementUtils.SimpleSetText(saleAreaElement, "Sale Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BADGE_MULTIPLY_TEXT", PassiveEventManager.Instance.GetEventInfoViewMultiplier(gemEventInfo)));

                MetaContextElementUtils.SetText(gemTextElement, StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_EVENT_TOTAL_GEM_TEXT", totalGem, eventTotalGem, price));

                // Debug.LogError(PassiveEventManager.Instance.GetEventInfoViewPercent(gemEventInfo));
                // Debug.LogError(PassiveEventManager.Instance.GetEventInfoViewMultiplier(gemEventInfo));
                // Debug.LogError(PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(gemEventInfo));
                saleAreaElement.gameObject.SetActive(true);
            }
            else
            {
                MetaContextElementUtils.SetText(gemTextElement, StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_TOTAL_GEM_TEXT", totalGem, price));

                saleAreaElement.gameObject.SetActive(false);
            }

            betAreaElement.gameObject.SetActive(false);
            gemTextElement.gameObject.SetActive(true);
        }

        private void PlayNowWithGem()
        {
            BlackboardUtils.SetOrCreateValue<string>(blackboard, "state", "OnPlayWithGem");

            UpdatePlayBetUI();

            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BUTTON_PLAY_NOW"));

            if(interactable)
                MetaContextElementUtils.SetClickable<ContextElement>( buttonElement,
                    ON_CLICK_BUTTON_EVENT,
                    agent,
                    ownerElement,
                    null
                );
        }

        private void PlayNowPurchaseGem()
        {
            BlackboardUtils.SetOrCreateValue<string>(blackboard, "state", "OnBuyProductWithPlay");

            UpdateBuyProductUI();

            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BUTTON_PLAY_NOW"));

            if(interactable)
                MetaContextElementUtils.SetClickable<ContextElement>( buttonElement,
                    ON_CLICK_BUTTON_EVENT,
                    agent,
                    ownerElement,
                    null
                );
        }

        private void BuyNow()
        {
            BlackboardUtils.SetOrCreateValue<string>(blackboard, "state", "OnBuyProduct");

            UpdateBuyProductUI();

            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BUTTON_BUY_NOW"));

            if(interactable)
                MetaContextElementUtils.SetClickable<ContextElement>( buttonElement,
                    ON_CLICK_BUTTON_EVENT,
                    agent,
                    ownerElement,
                    null
                );
        }

        private void ExceptionGoToShop()
        {
            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "IAM_AUTO_BALANCE_BUTTON_BUY_NOW"));

            // Open Shop. And Close. action..
            BlackboardUtils.SetOrCreateValue<string>(blackboard, "state", "OnOpenShop");

            betAreaElement.gameObject.SetActive(false);
            gemTextElement.gameObject.SetActive(true);
        }

        private long GetEventGem(long baseGem, EventInfo eventInfo)
        {
            if(eventInfo == null) return baseGem;
            if(eventInfo.type != EventInfoType.GEM_BAB_SHOP_EVENT_MULTIPLY) return baseGem;

            long totalGem = NumberUtils.GetMultiplierNumeratorValue(baseGem, PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo));

            return totalGem;
        }

        public void MakeBlackboardGemProduct()
        {
            if(selectProduct != null)
            {
                Blackboard componentBB = (Blackboard)BlackboardUtils.CreateBlackboard("iamComponentBB");
                componentBB.transform.SetParent(blackboard.transform);

                Blackboard action = (Blackboard)BlackboardUtils.CreateBlackboard("action");
                action.transform.SetParent(blackboard.transform);
                BlackboardUtils.SetOrCreateValue<ActionType>(action, "type", ActionType.PURCHASE_PRODUCT);
                BlackboardUtils.SetOrCreateValue<Blackboard>(action, "product", selectProduct);
                BlackboardUtils.SetOrCreateValue<int>(action, "userGroupId", 0);
                BlackboardUtils.SetOrCreateValue<Blackboard>(componentBB, "action", action);

                BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "_iamComponentBB", componentBB);
                BlackboardUtils.SetOrCreateValue<InAppMessageComponentType>(blackboard, "_componentType", InAppMessageComponentType.SINGLE_RANGE_BONUS_BUY_BUTTON);
            }
        }

        public void MakeBlackboardBuyBonusWithGemProduct()
        {
            // selectBonusInfo.bet + selectBonusInfo.extraBet;

            Blackboard action = (Blackboard)BlackboardUtils.CreateBlackboard("action");
            action.transform.SetParent(blackboard.transform);
            BlackboardUtils.SetOrCreateValue<int>(action, "gameId", gameId);
            BlackboardUtils.SetOrCreateValue<int>(action, "ticketedBonusId", ticketedBonusId);

            long bet = selectBonusInfo?.bet ?? 0L;
            long extraBet = selectBonusInfo?.extraBet ?? 0L;
            BlackboardUtils.SetOrCreateValue<long>(action, "bet", bet);
            BlackboardUtils.SetOrCreateValue<long>(action, "extraBet", extraBet);
            BlackboardUtils.SetOrCreateValue<ActionType>(action, "type", ActionType.BUY_BONUS);

            var product = ProductUtils.MakeBonusProduct(action, "buy_bonus");
            BlackboardUtils.SetOrCreateValue<long>(product, "gemPrice", targetTotalGem);

            BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "_action", action);
            BlackboardUtils.SetOrCreateValue<InAppMessageComponentType>(blackboard, "type", InAppMessageComponentType.SINGLE_RANGE_BONUS_BUY_BUTTON);
        }

        // private void StartPassive(EventData eventData)
        // {
        //     if(eventData.value != null)
        //     {
        //         EventInfoType eventType = (EventInfoType)eventData.value;
        //         if(EventInfoType.BONUS_SALE == eventType || EventInfoType.BONUS_MULTIPLY == eventType)
        //         {
        //             UpdateUI();
        //         }
        //     }
        // }

        // private void RefreshPassive(EventData eventData)
        // {
        //     if(eventData.value != null && bonusEventInfo != null)
        //     {
        //         int eventValueID = (int)eventData.value;
        //         if(bonusEventInfo.id == eventValueID)
        //         {
        //             UpdateUI();
        //         }
        //     }
        // }
    }
}
