using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Wheel")]
    public class SpinPBBigWheel : ActionTask<Transform>
    {
        public BBParameter<int> target;

        protected override string info { get { return string.Format("Spin to {0}", target); } }

        protected override void OnExecute()
        {
            var bigWheel = agent.GetComponent<PBBigWheel>();
            bigWheel.target = target.value;
            bigWheel.Spin();
            EndAction();
        }
    }
}
