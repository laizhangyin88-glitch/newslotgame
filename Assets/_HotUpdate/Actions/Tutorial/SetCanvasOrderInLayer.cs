using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine.UI;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class SetCanvasOrderInLayer : ActionTask<Transform>
    {
        public BBParameter<Transform> parent;
        public BBParameter<string> parentName;
        public BBParameter<int> order;
        public BBParameter<string> sortingLayerName;
        public BBParameter<bool> addGraphicRaycaster = false;
        
        protected override string info
        {
            get
            {
                Transform parent = this.parent != null && this.parent.value != null ? this.parent.value : agent;
                
                if (!String.IsNullOrEmpty(sortingLayerName.value))
                {
                    return string.Format("{0}.{1}.sortingOrder = ({2}, {3})",  parent, parentName, sortingLayerName, order);
                }
                else
                {
                    return string.Format("{0}.{1}.sortingOrder = {2}",  parent, parentName, order);
                }
            }
        }

        protected override void OnExecute()
        {
            Transform target;
            Transform parent;

            if (this.parent != null && this.parent.value != null)
            {
                parent = this.parent.value;
            }
            else
            {
                parent = agent;
            }

            if (!String.IsNullOrEmpty(parentName.value))
            {
                target = parent.Find(parentName.value);
            }
            else
            {
                target = parent;
            }

            if (target != null)
            {
                Canvas canvas = target.GetComponent<Canvas>();
    
                if (canvas == null)
                    canvas = target.gameObject.AddComponent<Canvas>();

                if (addGraphicRaycaster.value)
                    target.gameObject.AddComponent<GraphicRaycaster>();
                
                canvas.overrideSorting = true;
                canvas.sortingOrder = order.value;
    
                if (!String.IsNullOrEmpty(sortingLayerName.value))
                {
                    canvas.sortingLayerName = sortingLayerName.value;
                }
                
            }
            
            EndAction();
        }
    }
}
