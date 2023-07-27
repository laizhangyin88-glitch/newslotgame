using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using NodeCanvas.Framework.Internal;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("★ BagelCode")]
    public class CheckLegacyLobbyBottom : ConditionTask<Blackboard>
    {

        protected override string info
        {
            get { return "Check Legacy Lobby Bottom"; }
        }

        protected override bool OnCheck()
        {
            var bottomIconList = BlackboardUtils.FindVariable<List<Blackboard>>("/bottomIconList")?.value;
            return bottomIconList.Count == 0;
        }
    }
}
