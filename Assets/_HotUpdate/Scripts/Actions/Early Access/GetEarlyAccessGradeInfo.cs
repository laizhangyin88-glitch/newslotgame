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

public class GetEarlyAccessGradeInfo : ActionTask<Blackboard>
{
    public BBParameter<int> grade;
    public BBParameter<bool> isMaxGrade;

    public BBParameter<int> bonusSpinCount;

    protected override string info
    {
        get { return "Get Early Access Grade Info"; }
    }

    protected override void OnExecute()
    {
        if(isMaxGrade.value)
        {
            bonusSpinCount.value = BlackboardQueryUtils.GetEarlyAccessMaxCount(true);
        }
        else
        {
            bonusSpinCount.value = BlackboardQueryUtils.GetEarlyAccessMaxCount(grade.value);
        }

        EndAction();
    }
}

}
