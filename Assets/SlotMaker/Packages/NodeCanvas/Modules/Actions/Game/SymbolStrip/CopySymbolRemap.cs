using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Strip")]
    public class CopySymbolRemap : ActionTask
    {
        public BBParameter<SymbolRemap> from;
        public BBParameter<SymbolRemap> to;

        protected override string info
        {
            get { return string.Format("{0} = {1}", to, from); }
        }

        protected override void OnExecute()
        {
            to.value.remap = from.value.remap;
            EndAction();
        }
    }
}