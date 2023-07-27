using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/IAM")]
public class GetBonusIAM : ActionTask<Blackboard>
{
    public BBParameter<string> gameIDValue;

    public BBParameter<bool> isExist;
    public BBParameter<Blackboard> saveIamInfo;

    protected override string info
    {
        get { return string.Format("Get Buy a Bonus IAM Info({0})", gameIDValue); }
    }

    protected override void OnExecute()
    {
        isExist.value = false;
        saveIamInfo.value = null;

        var gameID = BlackboardUtils.FindVariable<int>(agent, gameIDValue.value);

        if(gameID != null)
        {
            Dictionary<int, Blackboard> bonusIAMDict = BlackboardQueryUtils.GetBonusIAMDict();

            if (bonusIAMDict.ContainsKey(gameID.value))
            {
                Blackboard iamInfo = bonusIAMDict[gameID.value];
                InAppMessageType iamType = iamInfo.GetValue<InAppMessageType>("type");
                
                isExist.value = true;
                saveIamInfo.value = iamInfo;

                Variable enabledMaxBet = BlackboardUtils.GetOrCreateVariable<bool>(null, "./enabledMaxBet");
                enabledMaxBet.value = false;

                Variable enabledBonus = null;
                
                if (iamType == InAppMessageType.SUPER_BONUS_PURCHASE_POPUP)
                    enabledBonus = BlackboardUtils.GetOrCreateVariable<bool>(null, "./enabledSuperBonus");
                else if (iamType == InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP)
                    enabledBonus = BlackboardUtils.GetOrCreateVariable<bool>(null, "./enabledBuyABonus");
                else if (iamType == InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP)
                    enabledBonus = BlackboardUtils.GetOrCreateVariable<bool>(null, "./enabledInstantBonus");

                if (enabledBonus != null)
                    enabledBonus.value = true;
                
                Variable bonusIamInfo = BlackboardUtils.GetOrCreateVariable<Blackboard>(null, "./bonusIamInfo");
                bonusIamInfo.value = iamInfo;
            }
        }

        EndAction();
    }
}

}
