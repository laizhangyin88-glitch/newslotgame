using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Logout : ActionTask
{
    protected override string info { get { return "Request Logout"; } }
    
    protected override void OnExecute()
    {
        BagelCodeClientAPI.SsoLogout(
        (response) =>
        {
            if(PlayerPrefs.HasKey("VALIDATE_EMAIL"))
                PlayerPrefs.DeleteKey("VALIDATE_EMAIL");
            
            EndAction(true);
        },
        (error) =>
        {
            EndAction(false);
        });
    }
}

}
