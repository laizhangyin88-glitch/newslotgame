using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{
    [Category("★ BagelCode/SlotMachine")]
    public class PlayAllSymbolAnimation : ActionTask
    {
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<string> animationName;

        protected override string info
        {
            get { return string.Format("Play all symbol {0} animation", animationName); }
        }

        protected override void OnExecute()
        {
            Blackboard bb = ContentBlackboard.Get();
            BaseSlotMachine slotMachine = BlackboardUtils.FindVariable<GameObject>(bb, "slotMachine").value.GetComponent<BaseSlotMachine>();

            for (int colIndex = 0; colIndex < column.value; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
                {
                    slotMachine.GetSymbol(colIndex, rowIndex).GetComponent<BaseSymbol>().Play(animationName.value);
                }
            }
            EndAction();
        }
    }

}
