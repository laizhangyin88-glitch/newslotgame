using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class AddSymbolsToSlot : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public BBParameter<GameObject> symbolPrefab;
        public BBParameter<List<int>> packedSpots;
        public MajorOrder packedMajorOrder = MajorOrder.ColumnMajor;
        public BBParameter<int> layer;

        protected override string info
        {
            get { return string.Format("{0}[{1}].AddSymbols({2}, {3})", mediator, layer, packedSpots, symbolPrefab); }
        }

        protected override void OnExecute()
        {
            int totalColumn = mediator.value.columnCount;
            int totalRow = mediator.value.rowCount;
            foreach (var packedSpot in packedSpots.value)
            {
                var spot = Cell.UnPack(totalColumn, totalRow, packedSpot, packedMajorOrder);
                var symbolInstance = SymbolInstancePool.GetSymbolInstance(symbolPrefab.value);
                symbolInstance.Initialize();
                mediator.value.AddSymbol(spot.column, spot.row, layer.value, symbolInstance);
                symbolInstance.gameObject.SetActive(true);
            }

            EndAction();
        }
    }
}