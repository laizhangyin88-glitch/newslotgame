using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode
{
    [Category("✫ Blackboard/Lists")]
    public class CreateOrClearList<T> : ActionTask where T : IList, new()
    {
        [RequiredField]
        [BlackboardOnly]
        public BBParameter<T> targetList;

        protected override string info
        {
            get { return string.Format("Clear List {0}", targetList); }
        }

        protected override void OnExecute()
        {
            if(targetList.value == null)
            {
                targetList.value = new T();
            }
            else
            {
                targetList.value.Clear();
            }

            EndAction(true);
        }
    }
}