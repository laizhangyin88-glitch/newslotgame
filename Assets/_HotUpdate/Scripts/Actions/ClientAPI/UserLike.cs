using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class UserLike : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;
    public BBParameter<int> like;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Like {0}", userId);
        } 
    }

    protected override void OnExecute()
    {
        if (userId.value != null)
        {
            BagelCodeClientAPI.UserLike(userId.value,
            (response) =>
            {
                if (response.error == BagelCode.ClientModels.Error.OK)
                {                   
                    like.value += 1;
                    EndAction(true);
                }
                else
                {
                    Debug.LogError(response.error);
                    EndAction(false);
                }                
            },
            (error) =>
            {
                EndAction(false);
            });
        }
        else
        {
            Debug.LogError("No UserId found." + agent.gameObject.name);
        }
    }
}

}
