using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class MainLogout : ActionTask
{
    protected override string info { get { return "Request Logout"; } }

    protected override void OnExecute()
    {

        BagelCodeClientAPI.Logout(
        (response) =>
        {
            if(PlayerPrefs.HasKey("VALIDATE_EMAIL"))
                PlayerPrefs.DeleteKey("VALIDATE_EMAIL");

            Internal.BagelCodeHTTP.Reset();

            EndAction(true);
        },
        (error) =>
        {
            EndAction(true);
        });
    }
}

}
