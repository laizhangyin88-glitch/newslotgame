using System;
using System.Linq;
using System.Collections.Generic;
using ParadoxNotion.Serialization;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using UnityEngine;

namespace SlotMaker
{
    [ParadoxNotion.Design.SpoofAOT]
    [CreateAssetMenu(fileName="New Blackboard", menuName="SlotMaker2/VariableAsset/NodeCanvas/Blackboard")]
    public class BlackboardAsset : ScriptableObject, ISerializationCallbackReceiver, IBlackboard
    {
        public event Action<Variable> onVariableAdded;
        public event Action<Variable> onVariableRemoved;

        [SerializeField]
        private string _serializedBlackboard = null;
        [SerializeField]
        private List<UnityEngine.Object> _objectReferences = null;

        [NonSerialized]
        private BlackboardSource _blackboard = new BlackboardSource();
        [NonSerialized]
        private bool hasDeserialized = false;

        //serialize blackboard variables to json
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
#if UNITY_EDITOR
            if ( JSONSerializer.applicationPlaying ) {
                return;
            }

            if ( _objectReferences != null && _objectReferences.Count > 0 && _objectReferences.Any(o => o != null) ) {
                hasDeserialized = false;
            }

            _objectReferences = new List<UnityEngine.Object>();
            _serializedBlackboard = JSONSerializer.Serialize(typeof(BlackboardSource), _blackboard, false, _objectReferences);
#endif
        }

        //deserialize blackboard variables from json
        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if ( hasDeserialized && JSONSerializer.applicationPlaying ) {
                return;
            }
            hasDeserialized = true;
            _blackboard = JSONSerializer.Deserialize<BlackboardSource>(_serializedBlackboard, _objectReferences);
            if ( _blackboard == null ) _blackboard = new BlackboardSource();
        }

        new public string name {
            get { return string.IsNullOrEmpty(_blackboard.name) ? this.name + "_BB" : _blackboard.name; }
            set
            {
                if ( string.IsNullOrEmpty(value) ) {
                    value = this.name + "_BB";
                }
                _blackboard.name = value;
            }
        }

        ///An indexer to access variables on the blackboard. It's recomended to use GetValue<T> instead
        public object this[string varName] {
            get { return _blackboard[varName]; }
            set { SetValue(varName, value); }
        }

        ///The raw variables dictionary. It's highly recomended to use the methods available to access it though
        public Dictionary<string, Variable> variables {
            get { return _blackboard.variables; }
            set { _blackboard.variables = value; }
        }

        ///The GameObject target to do variable/property binding
        public GameObject propertiesBindTarget {
            get { return null; }
        }

        ///----------------------------------------------------------------------------------------------

        [ContextMenu("Show Json")]
        void ShowJson() { JSONSerializer.ShowData(_serializedBlackboard, this.name); }

        ///----------------------------------------------------------------------------------------------

        ///Add a new variable of name and type
        public Variable AddVariable(string name, Type type) {
            var variable = _blackboard.AddVariable(name, type);
            if ( onVariableAdded != null ) {
                onVariableAdded(variable);
            }
            return variable;
        }

        ///Add a new variable of name and value
        public Variable AddVariable(string name, object value) {
            var variable = _blackboard.AddVariable(name, value);
            if ( onVariableAdded != null ) {
                onVariableAdded(variable);
            }
            return variable;
        }

        ///Delete the variable with specified name
        public Variable RemoveVariable(string name) {
            var variable = _blackboard.RemoveVariable(name);
            if ( onVariableRemoved != null ) {
                onVariableRemoved(variable);
            }
            return variable;
        }

        ///Get a Variable of name and optionaly type
        public Variable GetVariable(string name, Type ofType = null) {
            return _blackboard.GetVariable(name, ofType);
        }

        ///Get a Variable of ID and optionaly type
        public Variable GetVariableByID(string ID) {
            return _blackboard.GetVariableByID(ID);
        }

        //Generic version of get variable
        public Variable<T> GetVariable<T>(string name) {
            return _blackboard.GetVariable<T>(name);
        }

        ///Get the variable value of name
        public T GetValue<T>(string name) {
            return _blackboard.GetValue<T>(name);
        }

        ///Set the variable value of name
        public Variable SetValue(string name, object value) {
            return _blackboard.SetValue(name, value);
        }

        ///Get all variable names
        public string[] GetVariableNames() {
            return _blackboard.GetVariableNames();
        }

        ///Get all variable names of type
        public string[] GetVariableNames(Type ofType) {
            return _blackboard.GetVariableNames(ofType);
        }
    }
}