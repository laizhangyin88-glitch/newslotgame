using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class AddBonusEarnCreditNew : ActionTask<Blackboard>
    {
        public BBParameter<long> earnCredit;
        //public BBParameter<string> earnCredit;

        protected override string info
        {
            get { return string.Format("Add {0} to bonus new", earnCredit); }
        }

        protected override void OnExecute()
        {
            if (earnCredit != null)
            {
                addCredit(earnCredit.value);
            }
            else
            {
                Debug.LogError(string.Format("[AddBonusCredit] Cannot find ", earnCredit));
            }

            EndAction();
        }

        public static void addCredit(long credit)
        {
            var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus").value;
            if (credit > 0)
            {
                ContentBlackboardUtils.AddEarnCredit(bonus, credit);
            }
        }
    }

}
