using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Sirenix.Utilities;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class GetBlackboardValue<T> : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        [BlackboardOnly]
        public BBParameter<T> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = {1}", saveAs, valueA); }
        }

        protected override void OnExecute()
        {
            if(valueA.value.Contains("bonus/response/jackpotAwardAmount"))
            {
                Debug.LogError("=========== bonus/response/jackpotAwardAmount");
                Debug.LogError("===========================   " + agent.name + "     " + valueA.value);
            }
            var variable = BlackboardUtils.FindVariable<T>(agent, valueA.value);
            if (variable == null)
            {
                GraphOwner gOwner = null;
                if (ownerSystem != null) { 
                    gOwner = ownerSystem.agent.GetComponent<GraphOwner>();
                    Debug.LogError($"【agent】 {agent.gameObject.name} 【graph】 = {gOwner.graph.name}【GraphOwner】 = {gOwner.gameObject.name}");
                }
                Debug.LogError($"Blackboard of {agent.gameObject.name} , Null variable : {valueA.value} ; and save as : {saveAs.name}");
                //Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
                EndAction(false);
            }
            else
            {
                saveAs.value = variable.value;
                EndAction();
            }
        }
    }

}
