using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class WallOfEpicLike : ActionTask
{
    public BBParameter<int> id;
    
    protected override string info { get { return "Request WallOfEpic Like"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.WallOfEpicLike(id.value,
        (response) =>
        {
        },
        (error) =>
        {
        });

        EndAction(true);
    }
}

}
