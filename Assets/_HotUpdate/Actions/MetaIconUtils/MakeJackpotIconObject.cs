using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]

public class MakeJackpotIconObject : ActionTask<Blackboard>
{
    public BBParameter<string> jackpotInfoValue;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;

    public BBParameter<GameObject> saveAs;

    public BBParameter<bool> isDestroyPrevObject;

    protected override string info
    {
        get
        {
            return string.Format("{0} = Load Jackpot Icon", saveAs);
        }
    }

    protected override void OnExecute()
    {
        if(saveAs.value != null && isDestroyPrevObject.value)
            GameObject.Destroy(saveAs.value);

        var jackpotInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, jackpotInfoValue.value);

        if(jackpotInfoBB != null)
        {
            if(parent.value == null)
                parent.value = agent.transform;

            var jackpotAssetType = BlackboardUtils.FindVariable<JackpotAssetType>(jackpotInfoBB.value, "jackpotAssetType");
            saveAs.value = MetaIconUtils.MakeJackpotIconObject(jackpotAssetType.value, parent.value, parentName.value);
        }
        else
        {
            saveAs.value = null;
            Debug.Log("Jackpot Info is Null");
        }
        

        EndAction();
    }
}

}
