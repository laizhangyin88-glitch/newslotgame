using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetRandomGameID : ActionTask<Blackboard>
{
    [BlackboardOnly]
    public BBParameter<int> level;

    [BlackboardOnly]
    public BBParameter<int> tier;

    [BlackboardOnly]
    public BBParameter<int> ignoreGameID;

    [BlackboardOnly]
    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = Get Random Game ID", saveAs); }
    }

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetRandomGameID(ignoreGameID.value, level.value, tier.value);

        // Debug.LogError(saveAs.value);
        EndAction();
    }


}

}
