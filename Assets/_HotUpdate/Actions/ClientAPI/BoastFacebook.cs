using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class BoastFacebook : ActionTask
{
    protected override string info { get { return "Boast Facebook"; } }
    
    protected override void OnExecute()
    {
        BagelCodeClientAPI.ShareFacebook(
        (response) =>
        {
            Blackboard bb = agent.GetComponent<Blackboard>();
            ClientAPI2Blackboard.Serialize(bb, response);            
            
            EndAction(true);
        },
        (error) =>
        {
            EndAction(false);
        });
    }
}

}
