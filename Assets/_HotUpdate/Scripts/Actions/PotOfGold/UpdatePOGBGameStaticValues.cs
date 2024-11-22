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

[Category("★ BagelCode/Meta/POG")]
public class UpdatePOGBGameStaticValues : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> eventInfoBB;

    public BBParameter<Animator> saveAsWinJackpotAnimator;
    public BBParameter<int> saveAsResultIndex;
    public BBParameter<int> saveAsMinMultiplierIndex;

    private bool isInit = false;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private List<ContextElement> jackpotElementList;
    private List<Animator> jackpotAnimator;

    private Animator rootAnimator;

    private Blackboard itemBB;
    private List<double> jackpotMultiplierList;

    private const string JACKPOT_TEXT_FORMAT_KEY = "TEXT_POG_BOOSTER_JACKPOT_MULTIPLIER";
    private const string POPUP_POGB_JACKPOT_ORIG_MULTIPLIER = "POPUP_POGB_PURCHASE_JACKPOT_ORIG_MULTIPLIER";

    protected override void OnExecute()
    {
        saveAsResultIndex.value = -1;
        saveAsMinMultiplierIndex.value = 0;

        InitProperty();
        UpdateValues();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        jackpotElementList = new List<ContextElement>();
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Mini", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Minor", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Major", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Grand", ContextSearchingType.ChildrenSearch));

        jackpotAnimator = new List<Animator>();
        for(int i=0; i<jackpotElementList.Count; ++i)
        {
            jackpotAnimator.Add(jackpotElementList[i].gameObject.GetComponent<Animator>());
        }

        var itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "purchaseResponse/itemUseResultList").value;        
        saveAsResultIndex.value = itemUseResultList[0].GetValue<int>("multiplierIndex");
        saveAsWinJackpotAnimator.value = jackpotAnimator[saveAsResultIndex.value];

        var productID = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "purchaseResponse/productID").value;
        var productBB = BlackboardQueryUtils.GetProductFromID(productID);
        var itemBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.POG_BOOSTER);

        jackpotMultiplierList = new List<double>();
        List<Blackboard> jackpotMultiplierListBB = itemBB.GetValue<List<Blackboard>>("setting");

        for(int i=0; i<jackpotMultiplierListBB.Count; ++i)
        {
            long numerator = jackpotMultiplierListBB[i].GetValue<long>("multiplierNumerator");
            jackpotMultiplierList.Add( NumberUtils.GetMultiplierFromNumerator(numerator) );
        }

        isInit = true;
    }

    private void UpdateValues()
    {
        if(eventInfoBB.value != null)
        {
            var eventType = eventInfoBB.value.GetValue<EventInfoType>("type");

            switch(eventType)
            {
                case EventInfoType.POG_BOOSTER_MULTIPLY:
                    SetAllMultiplierValues();
                    break;
                case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                    SetMinMultiplierValues();
                    break;
                case EventInfoType.POG_BOOSTER_FREE:
                    SetMultiplierList(jackpotMultiplierList);
                    break;
                case EventInfoType.POG_BOOSTER_WITHOUT:
                    SetWithoutMultiplierValues();
                    SetMultiplierList(jackpotMultiplierList);
                    break;
            }
        }
        else
        {
            SetMultiplierList(jackpotMultiplierList);
        }
    }

    private void SetAllMultiplierValues()
    {
        long numerator = eventInfoBB.value.GetValue<long>("multiplierNumerator");
        double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(numerator);

        List<double> eventMultiplierList = new List<double>(jackpotMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            eventMultiplierList[i] *= eventMultiplier;
            jackpotAnimator[i].SetTrigger("Multiplier");
        }

        SetMultiplierList(eventMultiplierList);
        SetEventMultiplierList(jackpotMultiplierList);
    }

    private void SetMinMultiplierValues()
    {
        long numerator = eventInfoBB.value.GetValue<long>("multiplierNumerator");
        double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(numerator);

        List<double> eventMultiplierList = new List<double>(jackpotMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            if(eventMultiplierList[i] < eventMultiplier)
            {
                eventMultiplierList[i] = eventMultiplier;
                jackpotAnimator[i].SetTrigger("Multiplier");
            }
        }

        SetMultiplierList(eventMultiplierList);
        SetEventMultiplierList(jackpotMultiplierList);
    }

    private void SetWithoutMultiplierValues()
    {
        int eventIndex = eventInfoBB.value.GetValue<int>("minMultiplierIndex");
        saveAsMinMultiplierIndex.value = eventIndex;
        List<double> eventMultiplierList = new List<double>(jackpotMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            if(eventIndex > i)
                jackpotAnimator[i].SetTrigger("Without");
        }
    }

    private void SetMultiplierList(List<double> multiplierList)
    {
        MetaContextElementUtils.SimpleSetText(jackpotElementList[0], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[0]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[1], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[1]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[2], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[2]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[3], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[3]));
    }

    private void SetEventMultiplierList(List<double> multiplierList)
    {
        MetaContextElementUtils.SimpleSetText(jackpotElementList[0], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[0]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[1], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[1]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[2], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[2]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[3], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[3]));
    }
}

}

