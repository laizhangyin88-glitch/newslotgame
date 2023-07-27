using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{
    [Category("★ BagelCode/Blackboard")]
    public class DestroyBlackboard : ActionTask<Blackboard>
    {
        public BBParameter<string> bb;

        protected override string info
        {
            get { return string.Format("Destroy {0}", bb); }
        }

        protected override void OnExecute()
        {
            BlackboardUtils.DestroyBlackboard(agent, bb.value);
            EndAction();
        }
    }
}
