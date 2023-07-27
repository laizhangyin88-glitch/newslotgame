using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_free_bonus_popup : ActionTask
{
    protected override void OnExecute()
    {
        Blackboard purchaseResponse = BlackboardUtils.FindVariable<Blackboard>(null, "/purchaseResponse").value;
        List<Blackboard> itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/purchaseResponse/itemUseResultList").value;

        for (int i = 0; i < itemUseResultList.Count; ++i)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            ItemType itemType = BlackboardUtils.FindVariable<ItemType>(itemUseResultList[i], "itemType").value;;
            string typeValue = "coin";
            switch(itemType)
            {
            case ItemType.CREDIT:
                customData["type_of_reward"] = "coin";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<long>(itemUseResultList[i], "earnCredit").value;
                break;
            case ItemType.DAILY_BOOST:
                customData["type_of_reward"] = "daily_boost";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<long>(itemUseResultList[i], "earnCredit").value;
                typeValue = "dailyBoost";
                break;
            case ItemType.CREDIT_POT_OF_GOLD:
                customData["type_of_reward"] = "coin_pot_of_gold";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<long>(itemUseResultList[i], "earnCredit").value;
                typeValue = "pog";
                break;
            case ItemType.CREDIT_MULTIPLIER_WHEEL:
                customData["type_of_reward"] = "coin_booster";
                break;
            case ItemType.DAILY_BONUS_WHEEL:
                customData["type_of_reward"] = "daily_spin";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<int>(itemUseResultList[i], "addedSpinCount").value;
                typeValue = "dailySpin";
                break;
            case ItemType.PIGGY_BANK:
                customData["type_of_reward"] = "pot_of_gold";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<long>(itemUseResultList[i], "earnCredit").value;
                typeValue = "pog";
                break;
            case ItemType.CREDIT_WHEEL:
                customData["type_of_reward"] = "coin_wheel";
                customData["amount_of_reward"] = BlackboardUtils.FindVariable<long>(itemUseResultList[i], "earnCredit").value;
                break;
            default:
                break;
            }

            if (customData.Count > 0)
            {
                BiEventUtils.AppendLevelMultiplierEventData(customData, typeValue);
                Analytics.CustomEvent("client_free_bonus_popup", customData);
            }
        }

        EndAction();
    }
}

}
