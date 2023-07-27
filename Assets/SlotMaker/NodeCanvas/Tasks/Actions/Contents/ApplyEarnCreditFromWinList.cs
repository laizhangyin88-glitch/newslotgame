using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class ApplyEarnCreditFromWinList : ActionTask
    {
        public BBParameter<List<SymbolWin>> winList;

        protected override void OnExecute()
        {
            long totalEarnCredit = 0L;
            for (int i = 0; i < winList.value.Count; ++i)
            {
                var win = winList.value[i];
                totalEarnCredit += win.earnCredit;
            }

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList.value);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            EndAction();
        }
    }
}
