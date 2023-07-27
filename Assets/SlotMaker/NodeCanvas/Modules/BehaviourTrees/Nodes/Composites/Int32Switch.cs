using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;
using System;

namespace NodeCanvas.BehaviourTrees{

    [Name("Switch Int32 EX")]
    [Category("Composites/BagelCode")]
    [Description("Executes ONE child based on the provided string and return it's status. If 'case' change while a child is running, that child will be interrupted before the new child is executed")]
    [Icon("IndexSwitcher")]
    [Color("b3ff7f")]
    public class Int32Switch : BTComposite {

        [BlackboardOnly]
        public BBParameter<int> currentValue;
        public List<BBParameter<int>> valueList = new List<BBParameter<int>>();
        public int defaultIndex = -1;

        private int current;
        private int runningIndex;

        public override string name{
            get{return base.name.ToUpper();}
        }

        public override void OnChildConnected(int index){
            valueList.Insert(index, new BBParameter<int>{bb = graphBlackboard});
        }

        public override void OnChildDisconnected(int index){
            valueList.RemoveAt(index);
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard){
            if (outConnections.Count == 0)
                return Status.Failure;

            current = currentValue.value;

            int index = ContainsValue(current);

            if (runningIndex != index)
                outConnections[runningIndex].Reset();

            if (index == -1) {
                if (defaultIndex != -1) {
                    index = defaultIndex;
                } else {
                    return Status.Failure;
                }
            }

            status = outConnections[index].Execute(agent, blackboard);

            if (status == Status.Running)
                runningIndex = index;

            return status;
        }

        private int ContainsValue(int current)
        {
            for (int i = 0; i < valueList.Count; ++i)
            {
                if (valueList[i].value == current) return i;
            }

            return -1;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        public override void OnConnectionInspectorGUI(int i){
            bool isDefault = i == defaultIndex;
            valueList[i] = (BBParameter<int>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Case", valueList[i]);
            isDefault = GUILayout.Toggle(isDefault, "Default Case");
            if (isDefault)
                defaultIndex = i;
        }

        public override string GetConnectionInfo(int i){
            if (i == defaultIndex) {
                return "*Default*";
            } 
            else if (i >= valueList.Count)
            {
                return "*Impossible*";
            }
            else
            {
                return valueList[i].value.ToString();
            }
        }
        
        protected override void OnNodeGUI(){
            GUILayout.Label( currentValue.ToString() );
        }

        protected override void OnNodeInspectorGUI(){
            currentValue = (BBParameter<int>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Int", currentValue, true);
        }
        
        #endif
    }
}
