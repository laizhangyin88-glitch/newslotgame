using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class SuperBonusButton : MonoBehaviour
    {
        private Variable<int> gameId;
        
        private ContextElement agent;
        private ContextElement baseElement;
        private ContextElement baseText;
        private ContextElement baseEventText;
        private ContextElement saleTag;
        private ContextElement multiplyTag;

        private long endTimestamp;

        private void Awake()
        {
            gameId = BlackboardUtils.FindVariable<int>("./bonusIamInfo/gameId");
        }
        
        private void Start()
        {
            if (agent != null) return;

            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            baseElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);
            baseText = ContextUtils.FindElement(baseElement, "Text", ContextSearchingType.ChildrenSearch);
            baseEventText = ContextUtils.FindElement(baseElement, "Text Event", ContextSearchingType.ChildrenSearch);
            
            saleTag = ContextUtils.FindElement(agent, "Sale Tag", ContextSearchingType.ChildrenSearch);
            multiplyTag = ContextUtils.FindElement(agent, "Multiply Tag", ContextSearchingType.ChildrenSearch);
            
            
            if (gameId != null)
            {
                EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId.value);
                
                MetaContextElementUtils.SetActive(baseText, bonusEventInfo == null);
                MetaContextElementUtils.SetActive(baseEventText, bonusEventInfo != null);
                
                MetaContextElementUtils.SetActive(saleTag, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_SALE);
                MetaContextElementUtils.SetActive(multiplyTag, bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY);

                if (bonusEventInfo != null)
                {
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
    }
}
