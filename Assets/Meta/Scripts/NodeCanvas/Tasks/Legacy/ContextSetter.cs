using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

    [Name("Override Agent via Context")]
    [Category("Decorators/BagelCode")]
    [Description("[Legacy]Set another Agent for the rest of the Tree dynamicaly from this point and on. All nodes under this will be executed for the new agent")]
    [Icon("Set")]
    public class ContextSetter : BTDecorator{

        public BBParameter<SlotMaker.ContextElement> newAgent;

        protected override Status OnExecute(Component agent, IBlackboard blackboard){

            if (decoratedConnection == null)
                return Status.Resting;

            if (newAgent.value != null)
                agent = newAgent.value.transform;

            return decoratedConnection.Execute(agent, blackboard);
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        
        protected override void OnNodeGUI(){
            GUILayout.Label("Agent = " + newAgent);
        }

        #endif
    }
}
