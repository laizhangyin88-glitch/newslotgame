using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Output
{
    [Category("✶ Slots/Output")]
    public class ActiveOutputLayer : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutput> target;
        public BBParameter<int> layer;
        public BBParameter<bool> active;

        protected override string info
        {
            get { return string.Format("{0}[{1}].{2}()", target, layer, (active.value ? "Active" : "InActive")); }
        }

        protected override void OnExecute()
        {
            target.value.GetLayer(layer.value).active = active.value;
            EndAction();
        }
    }
}