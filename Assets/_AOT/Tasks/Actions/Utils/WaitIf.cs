using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class WaitIf : ActionTask
    {
        public BBParameter<float> waitTime = 1f;
        
        [BlackboardOnly]
        public BBParameter<int> valueA;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> valueB;

        protected override string info
        {
            get { return string.Format("IF " + valueA + OperationTools.GetCompareString(checkType) + valueB + " THEN Wait {0} sec.", waitTime); }
        }

        protected override void OnExecute()
        {
            if (!OperationTools.Compare(valueA.value, valueB.value, checkType))
                EndAction();
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= waitTime.value)
                EndAction();
        }
    }
}