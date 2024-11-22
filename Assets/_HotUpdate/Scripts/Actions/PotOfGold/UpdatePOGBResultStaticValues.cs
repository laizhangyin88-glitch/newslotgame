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
public class UpdatePOGBResultStaticValues : ActionTask<Blackboard> 
{
    public BBParameter<string> titleKey;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    protected override void OnExecute()
    {
        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        ContextElement titleTextElement = ContextUtils.FindElement(agentElement, "Title Area Full/Text", ContextSearchingType.FullNameSearch);
        ContextElement purchasedCoinTextElement = ContextUtils.FindElement(agentElement, "Purchased Coins/Text Coin", ContextSearchingType.FullNameSearch);
        ContextElement multiplierTextElement = ContextUtils.FindElement(agentElement, "Jackpot/Text Multiplier", ContextSearchingType.FullNameSearch);
        ContextElement jackpotIconElement = ContextUtils.FindElement(agentElement, "Jackpot/Text Multiplier/Jackpot Icon", ContextSearchingType.FullNameSearch);
        ContextElement totalCoinTextElement = ContextUtils.FindElement(agentElement, "Total Result/Text Coin", ContextSearchingType.FullNameSearch);
        ContextElement vipPointAreaElement = ContextUtils.FindElement(agentElement, "Vip Point", ContextSearchingType.ChildrenSearch);
        ContextElement vipPointTextElement = ContextUtils.FindElement(vipPointAreaElement, "Text Vip Point", ContextSearchingType.ChildrenSearch);
        ContextElement buttonElement = ContextUtils.FindElement(agentElement, "Button OK", ContextSearchingType.ChildrenSearch);

        Animator jackpotAnimator = jackpotIconElement.gameObject.GetComponent<Animator>();

        var itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "purchaseResponse/itemUseResultList").value;        
        var multiplier = itemUseResultList[0].GetValue<double>("multiplier");
        var multiplierIndex = itemUseResultList[0].GetValue<int>("multiplierIndex");
        var purchasedCoin = itemUseResultList[0].GetValue<long>("origEarnCredit");
        var totalEarnCoin = itemUseResultList[0].GetValue<long>("earnCredit");
        var earnRP = itemUseResultList[0].GetValue<long>("earnRp");

        MetaContextElementUtils.SetText(titleTextElement, StringTableUtils.GetString(tableType, titleKey.value));

        MetaContextElementUtils.SetText(purchasedCoinTextElement, StringTableUtils.GetString(tableType, "POPUP_POGB_RESULT_PREV_COIN_TEXT", purchasedCoin));
        MetaContextElementUtils.SetText(multiplierTextElement, StringTableUtils.GetString(tableType, "POPUP_POGB_RESULT_MULTIPLIER_TEXT", multiplier));
        MetaContextElementUtils.SetText(totalCoinTextElement, StringTableUtils.GetString(tableType, "POPUP_POGB_RESULT_TOTAL_COIN_TEXT", totalEarnCoin + purchasedCoin));

        vipPointAreaElement.gameObject.SetActive(earnRP > 0);
        if(earnRP > 0)
            MetaContextElementUtils.SetText(vipPointTextElement, StringTableUtils.GetString(tableType, "POPUP_COMMON_RP_TEXT", earnRP));

        jackpotAnimator.SetInteger("Jackpot", multiplierIndex);

        MetaContextElementUtils.SimpleSetText(buttonElement, "Text", StringTableUtils.GetString(tableType, "BUTTON_OK"));
        MetaContextElementUtils.SetClickable(
            buttonElement,
            "OnClose",
            false,
            false,
            SendEvent,
            ownerSystem
        );


        EndAction();
    }
}

}

