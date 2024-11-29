using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class Kudo : ActionTask<Blackboard> 
{
    public BBParameter<string> targetUserID;
    public BBParameter<int> kudoID;
    public BBParameter<ClientModels.KudoLikeType> likeType;
    public BBParameter<string> biKudoType;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Kudo {0}, {1}, {2}, {3}", targetUserID, kudoID, likeType, biKudoType);
        } 
    }

    protected override void OnExecute()
    {
        if(targetUserID != null && targetUserID.value != null)
        {
            BagelCodeClientAPI.Kudo(targetUserID.value, kudoID.value, likeType.value, biKudoType.value,
            (response) =>
            {
                if (response.error == ClientModels.Error.OK)
                {
                    // update earnRp
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                }
            },
            (error) =>
            {
            }
            );
        }

        EndAction(true);
    }
}

}
