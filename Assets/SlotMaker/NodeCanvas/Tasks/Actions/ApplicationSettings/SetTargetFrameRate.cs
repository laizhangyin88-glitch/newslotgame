using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/ApplicationSettings")]
    public class SetTargetFrameRate : ActionTask
    {
        public BBParameter<int> targetFrameRate;

        protected override string info { get { return "targetFrameRate = " + targetFrameRate; } }

        protected override void OnExecute()
        {
#if !UNITY_WEBGL
            Application.targetFrameRate = targetFrameRate.value;
#endif
            EndAction();
        }
    }
}
