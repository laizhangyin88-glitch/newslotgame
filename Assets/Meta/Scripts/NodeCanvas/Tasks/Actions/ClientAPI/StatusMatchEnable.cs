using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class StatusMatchEnable : ActionTask
{
    public BBParameter<int> deeplinkId;

    protected override string info { get { return "Enable StatusMatch"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.StatusMatchEnable(deeplinkId.value,
        (response) =>
        {
            // nothing todo
        },
        (error) =>
        {
            // nothing todo

        });
        EndAction(true);
    }
}

}
