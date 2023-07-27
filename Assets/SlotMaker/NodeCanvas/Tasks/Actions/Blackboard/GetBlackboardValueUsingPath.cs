using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;
using System;
namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    [Description("Use this for get value using path of value\n[Path Description]\n  [value name]: get value from self blackboard\n  ./[value name]: get value from content(global) blackboard\n  ./[turn]/[spin]/[value name]: get value from content(global) -> turn -> spin blackboard")]
    public class GetBlackboardValueUsingPath : ActionTask<Blackboard>
    {
        [RequiredField]
        public BBParameter<string> valueA;
        [BlackboardOnly]
        public BBObjectParameter saveAs;

        private Type prevType;
        private string typeName;
        protected override string info {
            get {
                    if(saveAs.varType != prevType) {
                        typeName = GetTypeName(saveAs.varType);
                        prevType = saveAs.varType;
                    }
                    return string.Format("[{0}] {1} = {2}", typeName, saveAs, valueA);
                }
        }

        protected override void OnExecute() {

            Type type = saveAs.varType;
            var variable = BlackboardUtils.FindVariable(agent, valueA.value, type);
            if (variable == null)
            {
                Debug.Log("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
                EndAction(false);
            }
            else
            {
                saveAs.value = variable.value;
                EndAction(true);
            }
        }

        public string GetTypeName(Type t, bool compileSafe = false) {
            if ( t == null ) {
                return null;
            }

            if (t.IsByRef ) {
                t = t.GetElementType();
            }

            if (t == typeof(UnityEngine.Object) ) {
                return "UnityObject";
            }

            var s = t.Name;
            if ( s == "Single" ) { s = "Float"; }
            if ( s == "Single[]" ) { s = "Float[]"; }
            if ( s == "Int32" ) { s = "Integer"; }
            if ( s == "Int32[]" ) { s = "Integer[]"; }

            if ( t.IsGenericParameter) {
                s = "T";
            }

            if ( t.IsGenericType) {
                s = t.Name;
                var args = t.GetGenericArguments();
                if ( args.Length != 0 ) {

                    s = s.Replace("`" + args.Length.ToString(), "");

                    s += "<";
                    for ( var i = 0; i < args.Length; i++ ) {
                        s += ( i == 0 ? "" : ", " ) + GetTypeName(args[i], compileSafe);
                    }
                    s += ">";
                }
            }

            return s;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {
            if ( GUILayout.Button("Select Target Variable Type") ) {
                EditorUtils.ShowPreferedTypesSelectionMenu(typeof(object), (t) => { saveAs.SetType(t); });
            }
            if ( saveAs.varType != typeof(object) ) {
                DrawDefaultInspector();
            }
        }

#endif
    }
}

