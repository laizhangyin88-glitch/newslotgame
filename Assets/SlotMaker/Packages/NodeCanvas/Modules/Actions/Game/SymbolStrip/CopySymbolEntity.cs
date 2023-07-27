using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Strip")]
    public class CopySymbolEntity : ActionTask
    {
        public SymbolEntity source;
        public List<SymbolEntity> targets;

        protected override void OnExecute()
        {
            for (int i = 0, count = targets.Count; i < count; ++i)
            {
                targets[i].Copy(source);
            }
            
            EndAction();
        }
    }
}