using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Utility")]
    public class ActiveWeightProgress : ActionTask
    {
        public BBParameter<object>progressObject;
        public BBParameter<bool> active;

        protected override string info
        {
            get
            {
                return string.Format("Weight Progress activeSelf = {0}", active);
            }
        }

        protected override void OnExecute()
        {
            //WeightProgress wp = progressObject.value as WeightProgress;
            //wp.UpdateProgress(key.value, progress.value);
            //EndAction();
        }
    }

}
