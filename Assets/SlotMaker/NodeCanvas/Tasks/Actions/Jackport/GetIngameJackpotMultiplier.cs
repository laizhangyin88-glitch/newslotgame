using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/JackpotChase")]
    public class GetIngameJackpotMultiplier : ActionTask<Blackboard>
    {
        public BBParameter<long> baseBet;
        public BBParameter<long> bet;

        // Save As
        public BBParameter<double> multiplier;

        protected override void OnExecute()
        {
            multiplier.value = (double)bet.value / (double)baseBet.value;

            EndAction();
        }
    }
}
