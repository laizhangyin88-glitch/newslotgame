using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class MoveSymbolsLayerToSlot : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public BBParameter<List<int>> packedSpots;
        public MajorOrder packedMajorOrder = MajorOrder.ColumnMajor;
        public BBParameter<int> sourceLayer;
        public BBParameter<int> targetLayer;

        protected override string info
        {
            get { return string.Format("{0}.MoveLayer({1} to {2})", mediator, sourceLayer, targetLayer); }
        }

        protected override void OnExecute()
        {
            int totalColumn = mediator.value.columnCount;
            int totalRow = mediator.value.rowCount;
            foreach (var packedSpot in packedSpots.value)
            {
                var spot = Cell.UnPack(totalColumn, totalRow, packedSpot, packedMajorOrder);
                mediator.value.MoveSymbolLayer(spot.column, spot.row, sourceLayer.value, targetLayer.value);
            }

            EndAction();
        }
    }
}