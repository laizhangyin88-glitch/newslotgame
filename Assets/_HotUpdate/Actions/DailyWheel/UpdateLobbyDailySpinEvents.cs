using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta/Daily Spin")]
public class UpdateLobbyDailySpinEvents : ActionTask<Blackboard> 
{
    public BBParameter<float> tagChangeDelay;
    public BBParameter<float> tagChangeSpd;

    private Animator rootAnimator;

    private ContextElement eventTagArea;
    private ContextElement eventTagElement;
    private ContextElement eventTagTextElement;
    private ContextElement eventTagRemainingTimerElement;

    private ContextElement eventTagLongElement;
    private ContextElement eventTagLongTextElement;
    private ContextElement eventTagLongRemainingTimerElement;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private const string TIME_FORMAT = "TIME_FORMAT_HHMMSS_TOTALHOUR";

    private bool isInit = false;

    protected override void OnExecute()
    {
        InitProperty();
        UpdateEvent();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        rootAnimator = agent.gameObject.GetComponent<Animator>();
        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        eventTagArea = ContextUtils.FindElement(agentElement, "Event Tag Area", ContextSearchingType.ChildrenSearch);

        eventTagElement = ContextUtils.FindElement(eventTagArea, "Event Tag", ContextSearchingType.ChildrenSearch);
        eventTagTextElement = ContextUtils.FindElement(eventTagElement, "Text", ContextSearchingType.ChildrenSearch);
        eventTagRemainingTimerElement = ContextUtils.FindElement(eventTagElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

        eventTagLongElement = ContextUtils.FindElement(eventTagArea, "Event Tag Long", ContextSearchingType.ChildrenSearch);
        if(eventTagLongElement == null)
            eventTagLongElement = MetaObjectUtils.MakePrefab<ContextElement>("Event Tag Long", eventTagArea.transform);
        eventTagLongElement.UpdateContext(false);

        eventTagLongTextElement = ContextUtils.FindElement(eventTagLongElement, "Text", ContextSearchingType.ChildrenSearch);
        eventTagLongRemainingTimerElement = ContextUtils.FindElement(eventTagLongElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

        isInit = true;
    }

    private void UpdateEvent()
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.DAILY_WHEEL_MULTIPLY);

        if(eventInfo != null)
        {
            EnableEvent(eventInfo);
        }
        else
        {
            DisableEvent();
        }
    }

    private void DisableEvent()
    {
        rootAnimator.SetBool("IsEvent", false);
        eventTagElement.gameObject.SetActive(false);
        eventTagLongElement.gameObject.SetActive(false);
    }

    private void EnableEvent(EventInfo eventInfo)
    {
        rootAnimator.SetBool("IsEvent", true);
        eventTagElement.gameObject.SetActive(false);
        eventTagLongElement.gameObject.SetActive(false);

        if(BlackboardQueryUtils.IsShopEventPercentText(ShopType.DAILY_BONUS))
        {
            eventTagLongElement.gameObject.SetActive(true);

            var numerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            long viewAddPercent = NumberUtils.GetAdditionalPercent(numerator);
            // MetaContextElementUtils.SetTextGlobal(eventTagLongTextElement, "TEXT_COMMON_PASSIVE_EVENT_ADDITIONAL_PERCENT", viewAddPercent);

            List<string> eventTagList = new List<string>();
            eventTagList.Add(StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_ADDITIONAL_PERCENT", viewAddPercent));

            // Blackboard eventTagBB = eventTagLongElement.gameObject.GetComponent<Blackboard>();
            // BlackboardUtils.SetOrCreateValue<List<string>>(eventTagBB, "eventTextList", eventTagList);
            // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeDelay", tagChangeDelay.value);
            // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeSpd", tagChangeSpd.value);

            // MetaContextElementUtils.SetCommonRemainingTimer( eventTagLongElement,
            //                                                  eventInfo.endTimestamp,
            //                                                  0,
            //                                                  TIME_FORMAT,
            //                                                  null,
            //                                                  null,
            //                                                  null,
            //                                                  true,
            //                                                  null
            //                                                 );

            EventTagController eventController = eventTagLongElement.gameObject.GetComponent<EventTagController>();

            // eventController.SetFlipTextData(eventTagList, tagChangeDelay.value, tagChangeSpd.value);
            eventController.Initialize( eventInfo.endTimestamp,
                                        TIME_FORMAT,
                                        null,
                                        null,
                                        true,
                                        null,
                                        eventTagList,
                                        tagChangeDelay.value,
                                        tagChangeSpd.value
                                    );
        }
        else
        {
            eventTagElement.gameObject.SetActive(true);

            var mutliplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
            // MetaContextElementUtils.SetTextGlobal(eventTagTextElement, "TEXT_DAILY_WHEEL_MULTIPLIER", mutliplier);

            // MetaContextElementUtils.SetCommonRemainingTimer( eventTagRemainingTimerElement,
            //                                                  eventInfo.endTimestamp,
            //                                                  0,
            //                                                  TIME_FORMAT,
            //                                                  null,
            //                                                  null,
            //                                                  null,
            //                                                  true,
            //                                                  null
            //                                                 );
            EventTagController eventController = eventTagElement.gameObject.GetComponent<EventTagController>();
            eventController.Initialize( eventInfo.endTimestamp,
                                        TIME_FORMAT,
                                        null,
                                        null,
                                        true,
                                        null,
                                        StringTableUtils.GetString(tableType, "TEXT_DAILY_WHEEL_MULTIPLIER", mutliplier)
                                    );
        }

        
    }
}

}
