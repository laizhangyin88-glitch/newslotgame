using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Math")]
    public class RandomRange : ActionTask 
    {
        public BBParameter<float> min;
        public BBParameter<float> max;

        [BlackboardOnly]
        public BBParameter<float> saveAs;

        protected override string info
        {
            get 
            {
                return string.Format("{0} = Random.Range({1}, {2}))", saveAs, min, max);
            }
        }

        protected override void OnExecute()
        {
            saveAs.value = UnityEngine.Random.Range(min.value, max.value);
            EndAction();
        }
    }
}
