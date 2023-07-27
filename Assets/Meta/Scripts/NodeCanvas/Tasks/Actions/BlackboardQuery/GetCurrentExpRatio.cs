using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/Me")]
public class GetCurrentExpRatio : ActionTask
{
    public BBParameter<float> expRatio;

    protected override string info
    {
        get { return string.Format("Get Current Exp Ratio as {0}", expRatio); }
    }

    protected override void OnExecute()
    {
        expRatio.value = BlackboardQueryUtils.GetCurrentExpRatio();

        EndAction();
    }
}

}
