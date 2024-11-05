using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Utils")]
public class GetLoginEntryGameInfo : ActionTask
{
    public BBParameter<Blackboard> saveAsGameInfoBB;
    public BBParameter<int>  saveAsGameId;
    public BBParameter<string> fromType;

    protected override string info
    {
        get {return string.Format("{0} = Get Login Entry GameInfo", saveAsGameInfoBB);}
    }

    protected override void OnExecute()
    {
        fromType.value = "";
        saveAsGameInfoBB.value = null;

        var enterGameID = BlackboardUtils.GetOrCreateVariable<int>( MainBlackboard.Get(), "enterGameId");
        saveAsGameId.value = enterGameID == null ? 0 : enterGameID.value;

        if(saveAsGameId.value > 0)
        {
            saveAsGameInfoBB.value = BlackboardQueryUtils.GetGameInfo(saveAsGameId.value);

            if(saveAsGameInfoBB.value != null && BlackboardQueryUtils.IsPlayableSlot(saveAsGameId.value))
            {
                fromType.value = "direct_first_launch";
            }
            else
            {
                saveAsGameInfoBB.value = null;
                saveAsGameId.value = 0;
            }
        }

        EndAction();
    }
}

}
