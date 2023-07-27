using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Strip")]
    public class RemapSymbol : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SymbolRemap> target;

        protected override string info
        {
            get { return string.Format("{0}.Remap()", target); }
        }

        protected override void OnExecute()
        {
            target.value.Remap();
            EndAction();
        }
    }
}