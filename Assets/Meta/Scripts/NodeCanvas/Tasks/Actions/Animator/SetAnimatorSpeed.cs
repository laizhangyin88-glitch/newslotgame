using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace BagelCode
{
    [Category("★ BagelCode/Animator")]
    public class SetAnimatorSpeed : ActionTask<Animator>
    {
        public BBParameter<float> speed;

        protected override string info
        {
            get { return string.Format("Mec.speed = {0}", speed); }
        }

        protected override void OnExecute()
        {
            agent.speed = speed.value;
            EndAction();
        }
    }
}