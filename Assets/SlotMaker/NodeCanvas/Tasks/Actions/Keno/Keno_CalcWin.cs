using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_CalcWin : ActionTask
    {
        public BBParameter<KenoMediator> mediator;
        public BBParameter<long> betCredit;
        public BBParameter<long> multiplier = 1;
        public BBParameter<KenoPaytable> kenoPaytable;
        public BBParameter<List<KenoWin>> saveAs;

        protected List<KenoWin> winList;
        [BlackboardOnly]
        public BBParameter<long> earnCredit;

        protected override void OnExecute()
        {
            List<KenoPay> paytable = kenoPaytable.value.paytable;

            winList = new List<KenoWin>();
            KenoWin kenoWin = new KenoWin();
            for (int i = 0; i < mediator.value.row * mediator.value.column; ++i)
            {
                var spot = mediator.value.GetSpot(i);
                if (spot.IsHit)
                    kenoWin.spots.Add(spot);
            }

            if (kenoWin.spots.Count > 0)
            {
                kenoWin.multiplier = multiplier.value;
                kenoWin.earnCredit = paytable[mediator.value.pickCount - 1].GetPay(kenoWin.spots.Count - 1) * betCredit.value * kenoWin.multiplier;
            }

            if (kenoWin.earnCredit > 0)
            {
                winList.Add(kenoWin);

                var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
                BlackboardUtils.SetOrCreateValue<List<KenoWin>>(spin, "winList", winList);
                ContentBlackboardUtils.AddEarnCredit(spin, kenoWin.earnCredit);

                earnCredit.value = kenoWin.earnCredit;

                saveAs.value = winList;                    
            }

            EndAction();
        }
    }
}
