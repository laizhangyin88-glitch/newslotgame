using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class AddfcfsTicket : ActionTask
{

    public BBParameter<string> encodedAction;
    protected override string info { get { return "Addfcfs Ticket"; } }
    
    protected override void OnExecute()
    {
        BagelCodeClientAPI.AddFCFSTicket(
        (response) =>
        {
            encodedAction.value = response.encodedAction;
            
            EndAction(true);
        },
        (error) =>
        {
            EndAction(true);
        });
    }
}

}
