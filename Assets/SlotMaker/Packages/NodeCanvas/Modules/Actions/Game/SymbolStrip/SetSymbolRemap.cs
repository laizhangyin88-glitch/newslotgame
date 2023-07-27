using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Strip")]
    public class SetSymbolRemap : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SymbolRemap> target;
        public BBParameter<string> path;

        protected override string info
        {
            get { return string.Format("{0} = {1}", target, path); }
        }

        protected override void OnExecute()
        {
            target.value.remap = BlackboardUtils.FindValue<Dictionary<int, int>>(path.value);            
            EndAction();
        }
    }
}