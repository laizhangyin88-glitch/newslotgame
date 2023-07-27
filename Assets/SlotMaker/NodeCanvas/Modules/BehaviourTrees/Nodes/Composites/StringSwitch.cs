using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;
using System;

namespace NodeCanvas.BehaviourTrees{

    [Name("Switch String EX")]
    [Category("Composites/BagelCode")]
    [Description("Executes ONE child based on the provided string and return it's status. If 'case' change while a child is running, that child will be interrupted before the new child is executed")]
    [Icon("IndexSwitcher")]
    [Color("b3ff7f")]
    public class StringSwitch : BTComposite {

        [BlackboardOnly]
        public BBParameter<string> currentValue;
        public List<BBParameter<string>> strings = new List<BBParameter<string>>();
        public int defaultIndex = -1;

        private string current;
        private int runningIndex;

        public override string name{
            get{return base.name.ToUpper();}
        }

        public override void OnChildConnected(int index){
            strings.Insert(index, new BBParameter<string>{bb = graphBlackboard});
        }

        public override void OnChildDisconnected(int index){
            strings.RemoveAt(index);
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

        private int ContainsValue(string current)
        {
            for (int i = 0; i < strings.Count; ++i)
            {
                if (strings[i].value == current) return i;
            }

            return -1;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        public override void OnConnectionInspectorGUI(int i){
            bool isDefault = i == defaultIndex;
            strings[i] = (BBParameter<string>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Case", strings[i]);
            isDefault = GUILayout.Toggle(isDefault, "Default Case");
            if (isDefault)
                defaultIndex = i;
        }

        public override string GetConnectionInfo(int i){
            if (i == defaultIndex) {
                return "*Default*";
            } 
            else if (i >= strings.Count)
            {
                return "*Impossible*";
            }
            else
            {
                if (strings[i] == null || strings[i].value == null || strings[i].value == "")
                    return "*Null*";
                else
                    return strings[i].value;
            }
        }
        
        protected override void OnNodeGUI(){
            GUILayout.Label( currentValue.ToString() );
        }

        protected override void OnNodeInspectorGUI(){
            currentValue = (BBParameter<string>)NodeCanvas.Editor.BBParameterEditor.ParameterField("String", currentValue, true);
        }
        
        #endif
    }
}
