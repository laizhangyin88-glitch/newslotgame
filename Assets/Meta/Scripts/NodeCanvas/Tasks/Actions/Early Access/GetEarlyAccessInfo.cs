using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/EarlyAccess")]

public class GetEarlyAccessInfo : ActionTask<Blackboard>
{
    public BBParameter<bool> saveAsIsAvailable;
    public BBParameter<int> saveAsGrade;
    public BBParameter<int> saveAsRemainTotalBonusSpinCount;
    public BBParameter<int> saveAsElaryAccessGameID;
    public BBParameter<GameType> saveAsEarlyAccessThumbGameType;

    protected override string info
    {
        get { return "Get Early Access Info"; }
    }

    protected override void OnExecute()
    {
        saveAsIsAvailable.value = BlackboardQueryUtils.IsEarlyAccessAvailable();
        saveAsElaryAccessGameID.value = BlackboardQueryUtils.GetEarlyAccessGameID();

        if(saveAsIsAvailable.value)
        {
            saveAsGrade.value = BlackboardQueryUtils.GetEarlyAccessGrade();
            saveAsRemainTotalBonusSpinCount.value = BlackboardQueryUtils.GetEarlyAccessRemainTotalSpinCount();
            saveAsEarlyAccessThumbGameType.value = BlackboardQueryUtils.GetGameType(saveAsElaryAccessGameID.value);
        }
        else
        {
            saveAsGrade.value = -1;
            saveAsRemainTotalBonusSpinCount.value = 0;
            saveAsEarlyAccessThumbGameType.value = GameType.UNKNOWN;
        }

        EndAction();
    }
}

}
