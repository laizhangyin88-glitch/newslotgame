using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Game
{
    [Category("✶ Slots/Game")]
    public class SetCredit : ActionTask
    {
        public ContentBlackboard.EntityType earnType = ContentBlackboard.EntityType.Spin;
        public OperationMethod operation = OperationMethod.Add;
        public BBParameter<long> earnCredit;

        protected override string info
        {
            get { return string.Format("{0} {1} {2}", earnType, OperationUtils.GetOperationString(operation), earnCredit); }
        }

        protected override void OnExecute()
        {
            switch (operation)
            {
            case OperationMethod.Add:
                    ContentBlackboardUtils.AddEarnCredit(ContentBlackboard.Get(earnType), earnCredit.value);
                break;
            case OperationMethod.Subtract:
                    ContentBlackboardUtils.AddSpentCredit(ContentBlackboard.Get(earnType), earnCredit.value);
                break;
            }
            EndAction();
        }
    }
}