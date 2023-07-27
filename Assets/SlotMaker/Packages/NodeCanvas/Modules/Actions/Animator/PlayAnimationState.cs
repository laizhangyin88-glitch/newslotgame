using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Animator")]
    public class PlayAnimationState : ActionTask<Animator>
    {
        public BBParameter<string> stateName;
        public BBParameter<int> layer = -1;
        public BBParameter<float> normalizedTime = float.NegativeInfinity;

        protected override string info
        {
            get { return string.Format("{0}.Play({1}, {2}, {3})", agent.name, stateName, layer, normalizedTime); }
        } 

        protected override void OnExecute()
        {
            agent.Play(stateName.value, layer.value, normalizedTime.value);
            EndAction();
        }
    }
}