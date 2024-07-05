using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/SlotMachine")]
    public class PlaySymbolAnimations : ActionTask<BaseSlotMachine>
    {
        public BBParameter<SymbolWin> symbolWin;
        public BBParameter<string> animationName;

        protected override string info
        {
            get { return string.Format("[Legacy]Play symbol {0} animation", animationName); }
        }

        protected override void OnExecute()
        {
            SymbolWin win = symbolWin.value;
            for (int i = 0; i < win.cells.Count; ++i)
            {
                var cell = win.cells[i];
                agent.GetSymbol(cell.column, cell.row).Play(animationName.value);
            }
            EndAction();
        }
    }
}
