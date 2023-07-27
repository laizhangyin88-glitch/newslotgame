using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/BossRaiders")]
    public class GetBossRaidersEnergy : ActionTask<Blackboard>
    {
        public BBParameter<long> saveAs;

        protected override void OnExecute()
        {
            saveAs.value = BossRaidersUtils.CurrentEnergy;
            EndAction();
        }
    }
}