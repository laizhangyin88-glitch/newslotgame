using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]

public class MakeRewardIconObject : ActionTask<Blackboard>
{
    public BBParameter<string> rewardTypeValue;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;

    public BBParameter<GameObject> saveAs;

    public BBParameter<bool> isDestroyPrevObject;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Load Reward Icon({1})", saveAs, rewardTypeValue);
        }
    }

    protected override void OnExecute()
    {
        if(saveAs.value != null && isDestroyPrevObject.value)
            GameObject.Destroy(saveAs.value);

        var rewardType = BlackboardUtils.FindVariable<RewardType>(agent, rewardTypeValue.value);

        if(rewardType != null)
        {
            if(parent.value == null)
                parent.value = agent.transform;

            saveAs.value = MetaIconUtils.MakeCommonRewardIconObject(rewardType.value, parent.value, parentName.value);
        }
        else
        {
            saveAs.value = null;
            Debug.Log("Reward Type is Null");
        }
        

        EndAction();
    }
}

}
