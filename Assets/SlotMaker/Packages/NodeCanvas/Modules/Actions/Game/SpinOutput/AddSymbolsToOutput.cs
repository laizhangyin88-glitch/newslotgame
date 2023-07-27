using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Output
{
    [Category("✶ Slots/Output")]
    public class AddSymbolsToOutput : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutput> target;
        public OverridenSymbolEntity symbol;
        public BBParameter<List<int>> packedSpots;
        public MajorOrder packedMajorOrder = MajorOrder.ColumnMajor;
        public BBParameter<int> layer;

        protected override string info
        {
            get { return string.Format("{0}[{1}].AddSymbols({2}, {3})", target, layer, packedSpots, symbol.master != null ? symbol.master.name : "[NULL]"); }
        }

        protected override void OnExecute()
        {
            int totalColumn = target.value.columnCount;
            int totalRow = target.value.rowCount;
            foreach (var packedSpot in packedSpots.value)
            {
                var spot = Cell.UnPack(totalColumn, totalRow, packedSpot, packedMajorOrder);
                target.value.SetSymbol(spot.column, spot.row, layer.value, symbol);
            }
            
            EndAction();
        }
    }
}