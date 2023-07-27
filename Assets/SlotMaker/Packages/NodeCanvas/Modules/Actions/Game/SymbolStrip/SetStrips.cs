using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker.Slots.Strategy;

namespace SlotMaker.Slots.Tasks.Actions.Strips
{
    [Category("✶ Slots/Strip")]
    public class SetStrips : ActionTask
    {
        public List<SymbolStrips> strips;
        public BBParameter<List<int>> remap;
        public SetStripsStrategy setStripsStrategy;

        protected override void OnExecute()
        {
            setStripsStrategy.Set(strips, remap.value);
            EndAction();
        }
    }
}