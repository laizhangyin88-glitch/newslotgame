using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Output
{
    [Category("✶ Slots/Output")]
    public class SetSpinOutput : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutput> target;
        public BBParameter<List<int>> indices;
        public BBParameter<int> layer;

        protected override string info
        {
            get { return string.Format("{0}[{1}].SetOutput({2})", target, layer, indices); }
        }

        protected override void OnExecute()
        {
            target.value.SetIndices(indices.value, layer.value);
            EndAction();
        }
    }
}