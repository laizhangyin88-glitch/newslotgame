using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class DecomposeCard : ActionTask
    {
        public BBParameter<int> cardNumber;

        public BBParameter<int> saveAsSuit;
        public BBParameter<int> saveAsNumber;

        protected override string info{
            get { return string.Format("Decompose Card {0} as suit and number", cardNumber); }
        }

        protected override void OnExecute()
        {
            saveAsSuit.value = cardNumber.value % 4;
            saveAsNumber.value = cardNumber.value / 13;

            EndAction();
        }
    }
}