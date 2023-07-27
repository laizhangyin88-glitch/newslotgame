using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Polling : ActionTask<Blackboard>
{
    [BlackboardOnly]
    public BBParameter<long> lastReceiveID;

    [BlackboardOnly]
    public BBParameter<bool> isError;

    protected override string info { get { return "Poll"; } }
    
    protected override void OnExecute()
    {
        BagelCodeClientAPI.Poll(lastReceiveID.value,
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);
            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "common"), response.common);

            isError.value = false;
            EndAction(true);
        },
        (error) =>
        {
            isError.value = true;
            EndAction(true);
        });
    }
}

}
