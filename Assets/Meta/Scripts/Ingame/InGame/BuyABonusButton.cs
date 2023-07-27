using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class BuyABonusButton : BuyABonusButtonBase
    {
        private Blackboard blackboard;

        private Variable<int> gameId;
        private long minGem;
        private long maxGem;
        private long winxNumerator;
        private long endTimestamp;
        private List<IAMUtils.BonusInfo> bonusInfoList;
        private bool rangeBonusBuyButtonFound;

        private bool useOneClickSpin;
        private InAppMessageComponentType oneClickIAMType;

        private long targetTotalGem;
        private int ticketedBonusId;

        public IAMUtils.BonusInfo selectBonusInfo = null;

        private EventInfo bonusEventInfo;

        private ContextElement agent;
        private ContextElement baseElement;
        private ContextElement baseText;
        private ContextElement baseEventText;
        private ContextElement gemBaseElement;
        private ContextElement gemBaseText;
        private ContextElement gemBaseEventText;
        private ContextElement gemBasePriceText;
        private ContextElement saleTag;
        private ContextElement multiplyTag;

        private Blackboard oneClickActionBB = null;

        private const string ON_CLICK_BAB = "OnClickBAB";
        private const string ON_PLAY_BONUS = "OnPlayBonus";
        private const string ON_TRIGGER_IAM = "OnTriggerIAM";

        private MessageDelegates passiveDelegates;
        private int bonusEventID = -1;

        protected override void Awake()
        {
            base.Awake();

            passiveDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "StartPassive", StartPassive },
                    { "RefreshPassive", RefreshPassive }
                }
            );
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            MessageDispatcher.Register("OnPassiveEvent", passiveDelegates.Delegate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MessageDispatcher.UnRegister("OnPassiveEvent", passiveDelegates.Delegate);
        }

        protected override void InitProperty()
        {
            blackboard = GetComponent<Blackboard>();
            if(blackboard == null)
                blackboard = gameObject.AddComponent<Blackboard>();

            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            baseElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);
            baseText = ContextUtils.FindElement(baseElement, "Text", ContextSearchingType.ChildrenSearch);
            baseEventText = ContextUtils.FindElement(baseElement, "Text Event", ContextSearchingType.ChildrenSearch);

            gemBaseElement = ContextUtils.FindElement(agent, "Gem Base", ContextSearchingType.ChildrenSearch);
            gemBaseText = ContextUtils.FindElement(gemBaseElement, "Text", ContextSearchingType.ChildrenSearch);
            gemBaseEventText = ContextUtils.FindElement(gemBaseElement, "Text Event", ContextSearchingType.ChildrenSearch);
            gemBasePriceText = ContextUtils.FindElement(gemBaseElement, "Price Text", ContextSearchingType.ChildrenSearch);

            saleTag = ContextUtils.FindElement(agent, "Sale Tag", ContextSearchingType.ChildrenSearch);
            multiplyTag = ContextUtils.FindElement(agent, "Multiply Tag", ContextSearchingType.ChildrenSearch);

            gameId = BlackboardUtils.FindVariable<int>("./bonusIamInfo/gameId");
            var componentList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonusIamInfo/componentList");

            rangeBonusBuyButtonFound = false;
            useOneClickSpin = BlackboardUtils.FindValue<bool>("/values/misc/ENABLE_BAB_ONE_CLICK_PURCHASE");

            oneClickIAMType = InAppMessageComponentType.UNKNOWN;

            if (gameId != null && componentList != null)
            {
                for (int i = 0; i < componentList.value.Count; i++)
                {
                    var componentType = componentList.value[i].GetValue<InAppMessageComponentType>("type");
                    if (componentType == InAppMessageComponentType.SINGLE_RANGE_BONUS_BUY_BUTTON)
                    {
                        minGem = componentList.value[i].GetValue<long>("minGem");
                        maxGem = componentList.value[i].GetValue<long>("maxGem");

                        ticketedBonusId = componentList.value[i].GetValue<int>("ticketedBonusId");
                        winxNumerator = BlackboardQueryUtils.GetWinXNumeratorFromTicketedBonusID(ticketedBonusId);

                        rangeBonusBuyButtonFound = true;
                        oneClickIAMType = componentType;

                        bonusInfoList = IAMUtils.GetBonusInfoList(minGem, maxGem, winxNumerator, gameId.value);
                        break;
                    }
                    else if (componentType == InAppMessageComponentType.RANGE_BONUS_BUY_BUTTON)
                    {
                        minGem = componentList.value[i].GetValue<long>("minGem");
                        maxGem = componentList.value[i].GetValue<long>("maxGem");

                        ticketedBonusId = componentList.value[i].GetValue<int>("ticketedBonusId");
                        winxNumerator = BlackboardQueryUtils.GetWinXNumeratorFromTicketedBonusID(ticketedBonusId);

                        rangeBonusBuyButtonFound = true;
                        oneClickIAMType = componentType;

                        bonusInfoList = IAMUtils.GetBonusInfoList(minGem, maxGem, winxNumerator, gameId.value);
                        break;
                    }
                }
            }

            MetaContextElementUtils.SetClickable(
                agent,
                ON_CLICK_BAB,
                agent,
                null
            );
        }

        public override void UpdateValues()
        {
            bonusEventID = -1;

            if(agent == null) return;

            if (gameId != null)
            {
                bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId.value);

                MetaContextElementUtils.SetActive(baseText, bonusEventInfo == null);
                MetaContextElementUtils.SetActive(baseEventText, bonusEventInfo != null);

                MetaContextElementUtils.SetActive(gemBaseText, bonusEventInfo == null);
                MetaContextElementUtils.SetActive(gemBaseEventText, bonusEventInfo != null);

                MetaContextElementUtils.SetActive(saleTag, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_SALE);
                MetaContextElementUtils.SetActive(multiplyTag, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY);

                if (bonusEventInfo != null)
                {
                    bonusEventID = bonusEventInfo.id;
                    long numerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);

                    EventTagController eventController = null;
                    string eventTagText = null;

                    if (bonusEventInfo.type == EventInfoType.BONUS_SALE)
                    {
                        eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INGAME_BONUS_BUTTON_SALE_TEXT", numerator);

                        eventController = saleTag.gameObject.GetComponent<EventTagController>();
                    }
                    else if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                    {
                        if(BlackboardQueryUtils.IsBuyABonusEventPercentText())
                        {
                            long viewAddPercent = NumberUtils.GetAdditionalPercent(numerator);
                            eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INGAME_BONUS_BUTTON_ADD_PERCENT_TEXT", viewAddPercent);
                        }
                        else
                        {
                            eventTagText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INGAME_BONUS_BUTTON_MULTIPLY_TEXT", NumberUtils.GetMultiplierFromNumerator(numerator));
                        }

                        eventController = multiplyTag.gameObject.GetComponent<EventTagController>();
                    }

                    if( eventController != null )
                    {
                        eventController.Initialize( bonusEventInfo.endTimestamp,
                                                    "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                                    null,
                                                    "Ended",
                                                    true,
                                                    null,
                                                    eventTagText
                            );
                    }
                }
            }
        }

        public override void RefreshButton()
        {
            if(agent == null) return;

            selectBonusInfo = null;

            if (gameId != null)
            {
                bool isGemBaseButton = false;

                if (rangeBonusBuyButtonFound)
                {
                    long betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                    for (int i = 0; i < bonusInfoList.Count; i++)
                    {
                        if (bonusInfoList[i].bet == betCredit)
                        {
                            isGemBaseButton = true;
                            selectBonusInfo = bonusInfoList[i];

                            // SetGem(bonusInfoList[i].gem);
                            break;
                        }
                    }

                    if(selectBonusInfo != null)
                    {
                        bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId.value);
                        long meGem = BlackboardUtils.FindVariable<long>(null, "/me/gem").value;

                        targetTotalGem = selectBonusInfo.gem;

                        if (bonusEventInfo != null)
                        {
                            if(bonusEventInfo.type == EventInfoType.BONUS_SALE)
                            {
                                targetTotalGem = NumberUtils.GetSaleNumeratorValue(targetTotalGem, PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo));
                                targetTotalGem = IAMUtils.RemoveUnitOfDigit(targetTotalGem);
                            }
                        }

                        string text = StringTableUtils.GetString(StringTable.StringTableType.Global, "INGAME_BONUS_BUTTON_GEM_PRICE", targetTotalGem);
                        MetaContextElementUtils.SetText(gemBasePriceText, text);

                        if(useOneClickSpin && meGem >= targetTotalGem)
                        {
                            MakeBlackboardGemProduct();

                            BlackboardUtils.SetOrCreateValue<string>(blackboard, "triggerStatus", ON_PLAY_BONUS);
                        }
                        else
                        {
                            BlackboardUtils.SetOrCreateValue<string>(blackboard, "triggerStatus", ON_TRIGGER_IAM);
                        }
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue<string>(blackboard, "triggerStatus", ON_TRIGGER_IAM);
                    }
                }

                MetaContextElementUtils.SetActive(baseElement, !isGemBaseButton);
                MetaContextElementUtils.SetActive(gemBaseElement, isGemBaseButton);
            }
        }

        public void MakeBlackboardGemProduct()
        {
            if(selectBonusInfo != null)
            {
                if(oneClickActionBB == null)
                {
                    oneClickActionBB = (Blackboard)BlackboardUtils.CreateBlackboard("action");
                    oneClickActionBB.transform.SetParent(blackboard.transform);
                }

                BlackboardUtils.SetOrCreateValue<int>(oneClickActionBB, "gameId", gameId.value);
                BlackboardUtils.SetOrCreateValue<int>(oneClickActionBB, "ticketedBonusId", ticketedBonusId);
                BlackboardUtils.SetOrCreateValue<long>(oneClickActionBB, "bet", selectBonusInfo.bet);
                BlackboardUtils.SetOrCreateValue<long>(oneClickActionBB, "extraBet", selectBonusInfo.extraBet);
                BlackboardUtils.SetOrCreateValue<ActionType>(oneClickActionBB, "type", ActionType.BUY_BONUS);

                var product = ProductUtils.MakeBonusProduct(oneClickActionBB, "buy_bonus");
                BlackboardUtils.SetOrCreateValue<long>(product, "gemPrice", targetTotalGem);

                BlackboardUtils.SetOrCreateValue<Blackboard>(blackboard, "_action", oneClickActionBB);
                BlackboardUtils.SetOrCreateValue<InAppMessageComponentType>(blackboard, "type", oneClickIAMType);
            }
        }

        private void StartPassive(EventData eventData)
        {
            if(eventData.value != null)
            {
                EventInfoType eventType = (EventInfoType)eventData.value;
                if(EventInfoType.BONUS_SALE == eventType || EventInfoType.BONUS_MULTIPLY == eventType)
                {
                    UpdateValues();
                    RefreshButton();
                }
            }
        }

        private void RefreshPassive(EventData eventData)
        {
            if(eventData.value != null)
            {
                int eventValueID = (int)eventData.value;
                if(bonusEventID == eventValueID)
                {
                    UpdateValues();
                    RefreshButton();
                }
            }
        }
    }
}
