using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions{

    [Category("★ BagelCode")]
    [Description("Will return true after a specific amount of time has passed and false if current time is less than timeout")]
    public class Timeover : ConditionTask {

        public BBParameter<float> timeover = 1f;
        private float currentTime;
        private Coroutine coroutine;

        protected override string info{
            get {return string.Format("Timeover {0} >= {1}", currentTime.ToString("0.00"), timeover.ToString());}
        }

        protected override void OnEnable()
        {
            if (coroutine == null){
                currentTime = 0;
                coroutine = StartCoroutine(Do());
            }
        }

        protected override void OnDisable(){
            if (coroutine != null){
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        protected override bool OnCheck()
        {
            if (currentTime >= timeover.value){
                return true;
            }
            return false;
        }

        IEnumerator Do(){
            while (currentTime < timeover.value){
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
