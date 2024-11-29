using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class MetaJackpotInfo : ActionTask <Blackboard> 
{
    public MetaJackpotType jackpotType;

    protected override string info 
    {
        get 
        {
            return "Request Meta Jackpot Info";
        }
    }

    protected override void OnExecute()
    {
#if NEW_NET
    EndAction(true);
    return; 
#endif
            if (jackpotType != MetaJackpotType.UNKNOWN)
        {
            BagelCodeClientAPI.MetaJackpotInfo(jackpotType,
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                //Debug.Log($"@A MetaJackpotResponseV1 = {oldJson}");

                BlackboardQueryUtils.UpdateMetaJackpotInfo(jackpotType, response.jackpotList, response.serverTime);
                EndAction(true);
            },
            (error) =>
            {
                EndAction(false);
            });
        }
        else
        {
            EndAction(false);
        }
    }
}

}
