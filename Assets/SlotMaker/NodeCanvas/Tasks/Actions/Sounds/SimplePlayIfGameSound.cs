using System.Collections;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Sounds")]
    public class SimplePlayIfIntGameSound : ActionTask
    {
        public BBParameter<string> id;
        
        [BlackboardOnly]
        public BBParameter<int> valueA;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> valueB;

        protected override string info { get { return "IF " + valueA + OperationTools.GetCompareString(checkType) + valueB + " THEN Play " + id; } }

        protected override void OnExecute()
        {
            if (OperationTools.Compare(valueA.value, valueB.value, checkType))
                GSManager.Instance.GetHandler(id.value).Play();

            EndAction();
        }
    }
}
