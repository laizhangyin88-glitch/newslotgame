using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class UpdateMysterySymbolMultipliers : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<string> mysterySymbolMultiplierPath = "./spin/response/result/mysterySymbols";

        protected override void OnExecute()
        {
            Dictionary<int, int> mysterySymbolMultipliers = BlackboardUtils.FindVariable<Dictionary<int, int>>(agent, mysterySymbolMultiplierPath.value).value;
            var mysterySymbolTable = ContentCustomData.GetSlotData(slotIndex.value).mysterySymbolTable;

            foreach (int key in mysterySymbolMultipliers.Keys)
            {
                mysterySymbolTable.SetSymbolMultiplier(0, key, mysterySymbolMultipliers[key]);
            }

            EndAction();
        }
    }

}
