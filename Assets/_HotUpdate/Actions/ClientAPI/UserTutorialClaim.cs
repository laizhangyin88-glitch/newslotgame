using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class UserTutorialClaim : ActionTask
{
    public BBParameter<string> key;

    protected override void OnExecute()
    {
        // BagelCodeClientAPI.UserTutorialClaim(key.value,
        // (response) =>
        // {
        //     BlackboardQueryUtils.AddCoins(response.earnCredit);
        //     EndAction(true);
        // },
        // (error) =>
        // {
        //     GlobalErrorHandler.GlobalError(error);
        // });
    }
}

}
