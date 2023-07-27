using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Output
{
    [Category("✶ Slots/Output")]
    public class ResetOutputLayer : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutput> target;
        public BBParameter<int> layer;

        protected override string info
        {
            get { return string.Format("{0}[{1}].Reset()", target, layer); }
        }

        protected override void OnExecute()
        {
            target.value.GetLayer(layer.value).Reset();
            EndAction();
        }
    }
}