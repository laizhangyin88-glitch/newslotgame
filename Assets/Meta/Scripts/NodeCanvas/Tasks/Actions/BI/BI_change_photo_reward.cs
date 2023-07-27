using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_change_photo_reward : ActionTask
{
    public BBParameter<long> earnCoin;

    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_change_photo_reward", new Dictionary<string, object>
        {
            { "earn_coin", earnCoin.value }
        });

        EndAction();
    }
}

}
