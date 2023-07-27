using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif


namespace NodeCanvas.BehaviourTrees{

    [Name("Switch Enum EX 1")]
    [Category("Composites/BagelCode")]
    [Description("Executes ONE child based on the provided int or enum and return it's status. If 'case' change while a child is running, that child will be interrupted before the new child is executed")]
    [Icon("IndexSwitcher")]
    [Color("b3ff7f")]
    public class EnumSwitch1 : BTComposite {

        [BlackboardOnly]
        public BBObjectParameter enumCase = new BBObjectParameter(typeof(System.Enum));
        public List<BBParameter<int>> cases = new List<BBParameter<int>>();
        public Type selectedType;
        public int defaultIndex = -1;

        private int current;
        private int runningIndex;
        private Type[] allTypes;
        private List<Type> enumTypes;
        private List<string> typeNames;

        public override string name{
            get{return base.name.ToUpper();}
        }

        public override void OnChildConnected(int index){
            
            if (cases.Count == 0) {
                cases.Insert(index, new BBParameter<int>{value=0, bb = graphBlackboard});
            } else {
                cases.Insert(index, new BBParameter<int>{value=cases[cases.Count-1].value+1, bb = graphBlackboard});
            }

            // var enumNames = System.Enum.GetNames(enumCase.value.GetType());
            // cases.Insert(index, new BBObjectParameter(enumCase.value.GetType()){value=1, bb = graphBlackboard});
            // cases[index].SetType(enumCase.value.GetType());
        }

        public override void OnChildDisconnected(int index){
            cases.RemoveAt(index);
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard){

            if (outConnections.Count == 0)
                return Status.Failure;

            //current = (int)System.Enum.Parse(enumCase.value.GetType(), enumCase.value.ToString());
            current = (int)enumCase.value;

            int index = ContainsValue(current);

            if (index == -1) {
                if (defaultIndex != -1) {
                    index = defaultIndex;
                } else {
                    return Status.Failure;
                }
            }

            if (runningIndex != index)
                outConnections[runningIndex].Reset();

            status = outConnections[index].Execute(agent, blackboard);

            if (status == Status.Running)
                runningIndex = index;

            return status;
        }

        private int ContainsValue(int current)
        {
            for (int i = 0; i < cases.Count; ++i)
            {
                if (cases[i].value == current) return i;
            }

            return -1;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        public override void OnConnectionInspectorGUI(int i){
            bool isDefault = i == defaultIndex;
            // var enumNames = System.Enum.GetNames(enumCase.value.GetType());                      
            //if (enumTypes != null && selectedType > 0) {
            if (selectedType != null) {
                var enumNames = System.Enum.GetNames(selectedType);
                var enumNumbers = System.Enum.GetValues(selectedType);
                List<int> enumNumbersList = new List<int>();
                int relativeIdx = 0;
                foreach (int t in enumNumbers) {
                    enumNumbersList.Add(t);
                }
                GUILayout.Label("Case");
                relativeIdx = enumNumbersList.IndexOf(cases[i].value);
                if (relativeIdx == -1) relativeIdx = 0;
                relativeIdx = EditorGUILayout.Popup(relativeIdx, enumNames);
                cases[i].value = (int)enumNumbersList[relativeIdx];
            } else {
                cases[i] = (BBParameter<int>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Case", cases[i]);
            }
            isDefault = GUILayout.Toggle(isDefault, "Default Case");
            if (isDefault)
                defaultIndex = i;
        }

        public override string GetConnectionInfo(int i){
            if (i == defaultIndex) {
                return "*Default*";
            } 
            Type _enumType;
            /*
            if (enumTypes != null && selectedType > 0 && selectedType < enumTypes.Count) {
                _enumType = enumTypes[selectedType];
                */
            if (selectedType != null) {
                _enumType = selectedType;
            } else if (enumCase.value == null) {
                return "*Null Enum*";
            } else {
                _enumType = enumCase.value.GetType();
            }
            var enumNames = System.Enum.GetNames(_enumType);
            var enumNumbers = System.Enum.GetValues(_enumType);
            List<int> enumNumbersList = new List<int>();
            int relativeIdx = 0;
            if (i >= cases.Count) {
                return "*Out of Case*"; //i.ToString();
            }
            foreach (int t in enumNumbers) {
                enumNumbersList.Add(t);
            }
            relativeIdx = enumNumbersList.IndexOf(cases[i].value);
            if (relativeIdx == -1)
            {
                return "*Not Existed Enum*";
            }
            else
            {
                return enumNames[relativeIdx];
            }
        }
        
        protected override void OnNodeGUI(){
            GUILayout.Label( enumCase.ToString() );
            /*
            if (enumTypes == null) {
                allTypes = Assembly.GetExecutingAssembly().GetTypes();
                enumTypes = new List<Type>();
                typeNames = new List<string>();
                enumTypes.Add(typeof(System.Enum));
                typeNames.Add(typeof(System.Enum).ToString());
                for (int i = 0; i < allTypes.Length; i++) {
                    if (allTypes[i].IsSubclassOf(typeof(System.Enum))) {
                        enumTypes.Add(allTypes[i]);
                        typeNames.Add(allTypes[i].ToString());
                    }
                }
            }
            */
        }

        protected override void OnNodeInspectorGUI(){
            DrawDefaultInspector();
            if (GUILayout.Button("Select Type"))
            {
                EditorUtils.ShowPreferedTypesSelectionMenu(typeof(System.Enum), (t) => { selectedType = t; });
            }

            /*
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enum Type");
            selectedType = EditorGUILayout.Popup(selectedType, typeNames.ToArray());
            GUILayout.EndHorizontal();

            Type usedEnum = null;
            enumCase = (BBObjectParameter)NodeCanvas.Editor.BBParameterEditor.ParameterField("Enum", enumCase, true);
            if (enumCase.value != null){
                usedEnum = enumCase.value.GetType();
            } else if (selectedType > 0) {
                usedEnum = enumTypes[selectedType];
            }
            if (usedEnum != null){
                GUILayout.BeginVertical("box");
                foreach (var s in System.Enum.GetNames(usedEnum) )
                    GUILayout.Label(s);
                GUILayout.EndVertical();
            }
            */
        }
        
        #endif
    }
}
