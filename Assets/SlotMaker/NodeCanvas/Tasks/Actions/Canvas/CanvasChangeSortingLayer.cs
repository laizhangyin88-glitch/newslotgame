using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Canvas")]
    public class CanvasChangeSortingLayer : ActionTask<Transform>
    {
        public BBParameter<string> sortingLayerName;
        public BBParameter<int> sortingOrder;

        private bool changeOrder;

        protected override string info {
            get { return "Canvas Change Sorting Layer " + sortingLayerName + ", " + sortingOrder; }
        }

        protected override void OnExecute()
        {
            changeOrder = !sortingOrder.isNone && !sortingOrder.isNull;

            var canvas = agent.GetComponent<Canvas>();
            Change(canvas);

            EndAction();
        }

        private void Change(Canvas canvas)
        {
            canvas.sortingLayerName = sortingLayerName.value;
            if (changeOrder)
                canvas.sortingOrder = sortingOrder.value;
        }
    }
}
