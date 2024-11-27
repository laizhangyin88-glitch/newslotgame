using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Name("Set Integer Random")]
    [Category("✫ Blackboard")]
    [Description("Set a blackboard integer variable at random between min and max value")]
    public class SetIntRandom : ActionTask
    {

        public BBParameter<int> minValue;
        public BBParameter<int> maxValue;

        private int index = 0;

        [BlackboardOnly]
        public BBParameter<int> intVariable;

        protected override string info
        {
            get { return "Set " + intVariable + " Random(" + minValue + ", " + maxValue + ")"; }
        }

        protected override void OnExecute()
        {
            if (LastFreeGameManager.Instance.isLastGameSpin)
            {
                List<int> list = LastFreeGameManager.Instance.GetKENOIndeices();
                if(list.Count > 0 && list.Count > index)
                {
                    intVariable.value = list[index];
                    Debug.LogError(intVariable.value);
                    index++;
                }
            }
            else
            {
                intVariable.value = Random.Range(minValue.value, maxValue.value + 1);

            }
            EndAction();
        }
    }
}
