using UnityEngine;
using UnityEngine.Rendering;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SortingGroup")]
    public class SetSortingGroup : ActionTask<Transform>
    {
        public BBParameter<string> sortingLayerName;
        public BBParameter<int> sortingOrder;

        private bool changeOrder;

        protected override string info {
            get { return "Set Sorting Group " + sortingLayerName + ", " + sortingOrder; }
        }

        protected override void OnExecute()
        {
            changeOrder = !sortingOrder.isNone && !sortingOrder.isNull;

            var sortingGroup = agent.GetComponent<SortingGroup>();
            Change(sortingGroup);

            EndAction();
        }

        private void Change(SortingGroup sortingGroup)
        {
            sortingGroup.sortingLayerName = sortingLayerName.value;
            if (changeOrder)
                sortingGroup.sortingOrder = sortingOrder.value;
        }
    }
}
