using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine.UI;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class SetImageAlpha : ActionTask
    {
        public BBParameter<Transform> parent;
        public BBParameter<string> parentName;
        public BBParameter<Color> color;
        
        protected override string info
        {
            get
            {
                return string.Format("{0}.{1}.color = {2}",  parent, parentName, color);
            }
        }

        protected override void OnExecute()
        {
            Transform target;

            if (!String.IsNullOrEmpty(parentName.value))
            {
                target = parent.value.Find(parentName.value);
            }
            else
            {
                target = parent.value;
            }

            target.GetComponent<Image>().color = color.value;
            
            EndAction();
        }
    }
}
