using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Slots.Tasks.Actions
{
    [Category("✶ Slots/Events")]
    public class DispatchVariableAsset : ActionTask
    {
        public BBParameter<VariableAsset> asset;

        protected override string info
        {
            get { return string.Format("{0}.Dispatch()", asset); }
        }

        protected override void OnExecute()
        {
            asset.value.Dispatch();
            EndAction();
        }
    }
}