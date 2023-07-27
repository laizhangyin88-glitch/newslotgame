using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Cards;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class SetPokerWin : ActionTask<Blackboard> 
    {
        public BBParameter<PokerWin> pokerWin;

        public BBParameter<int> owner;
        public BBParameter<int> paytableIndex;
        public BBParameter<long> earnCredit;
        public BBParameter<long> multiplier;

        protected override void OnExecute()
        {
            owner.value = pokerWin.value.owner;
            paytableIndex.value = pokerWin.value.paytableIndex;
            earnCredit.value = pokerWin.value.earnCredit;
            multiplier.value = pokerWin.value.multiplier;

            EndAction();
        }
    }
}