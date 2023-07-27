using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class InitMetaGameSlotMachineExpectation : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;

        protected override void OnExecute()
        {
            var newExpectation = new Expectation();
            newExpectation.expectations = new List<bool>();
            newExpectation.expectationSpots = new List<List<Cell>>();
            for (int i = 0; i < column.value; ++i)
            {
                newExpectation.expectations.Add(false);
                newExpectation.expectationSpots.Add(new List<Cell>());
            }

            MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value).expectation = newExpectation;

            EndAction();
        }
    }
}