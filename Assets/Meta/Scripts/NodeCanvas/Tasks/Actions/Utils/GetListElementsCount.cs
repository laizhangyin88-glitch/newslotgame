using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode")]
    public class GetListElementsCount<T> : ActionTask<Blackboard>
    {
        public BBParameter<List<T>> list;

        public BBParameter<int> saveValue;

        protected override string info
        {
            get { return string.Format("{0} = Count of {1}", saveValue, list); }
        }

        protected override void OnExecute()
        {
            if(list == null || list.value == null)
            {
                saveValue.value = 0;
            }
            else
            {
                saveValue.value = list.value.Count;
            }

            EndAction();
        }
    }
}