using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/SlotMachine")]
    public class PlaySymbolAnimationByCell : ActionTask<BaseSlotMachine>
    {
        public BBParameter<Cell> cell;
        public BBParameter<string> animationName;
        
        protected override string info
        {
            get { return string.Format("Play symbol {0} animation", animationName); }
        }

        protected override void OnExecute()
        {
            agent.GetSymbol(cell.value.column, cell.value.row).Play(animationName.value);
            EndAction();
        }
    }
}
