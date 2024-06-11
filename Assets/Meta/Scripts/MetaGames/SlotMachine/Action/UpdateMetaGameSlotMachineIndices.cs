using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class UpdateMetaGameSlotMachineIndices : ActionTask
    {
        public BBParameter<GameObject> slotMachine;
        public BBParameter<List<int>> indices;
        public BBParameter<int> offset;

        protected override void OnExecute()
        {
            var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
            int totalRow = MetaSlotMachineContentCustomData.GetSlotData(sm.slotIndex).row;

            sm.SetStripIndices(indices.value, totalRow + offset.value);

            EndAction();
        }
    }
}
