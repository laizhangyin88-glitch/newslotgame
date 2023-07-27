using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;
using NodeCanvas.Framework;
using ParadoxNotion.Serialization;
using Sirenix.OdinInspector;

namespace SlotMaker.Extentions
{
	[AddComponentMenu("SlotMaker/NodeCanvas/Standalone Action List (SlotMaker)")]
	public class ActionListPlayer : MonoBehaviour, ITaskSystem, ISerializationCallbackReceiver
	{
	    public EnableAction enableAction = EnableAction.DoNothing;

	    protected bool startCalled;

	    protected virtual void Start()
	    {
	    	startCalled = true;
	    	if (enableAction == EnableAction.EnableBehaviour)
	    		Play();
	    }

	    protected virtual void OnEnable()
	    {
	    	if (startCalled && enableAction == EnableAction.EnableBehaviour)
	    		Play();
	    }

        [SerializeField]
        private string _serializedList;
        [SerializeField]
        private List<UnityEngine.Object> _objectReferences;

        [SerializeField]
        private Blackboard _blackboard;

        [System.NonSerialized]
        private ActionList _actionList;

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
#if UNITY_EDITOR
            if ( JSONSerializer.applicationPlaying ) {
                return;
            }
            _objectReferences = new List<UnityEngine.Object>();
            _serializedList = JSONSerializer.Serialize(typeof(ActionList), _actionList, false, _objectReferences);
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            _actionList = JSONSerializer.Deserialize<ActionList>(_serializedList, _objectReferences);
            if ( _actionList == null ) _actionList = (ActionList)NodeCanvas.Framework.Task.Create(typeof(ActionList), this);
        }


        ///----------------------------------------------------------------------------------------------

        public ActionList actionList {
            get { return _actionList; }
        }

        Component ITaskSystem.agent {
            get { return this; }
        }

        public IBlackboard blackboard {
            get { return _blackboard; }
            set
            {
                if ( !ReferenceEquals(_blackboard, value) ) {
                    _blackboard = (Blackboard)(object)value;
                    SendTaskOwnerDefaults();
                }
            }
        }

        public float elapsedTime {
            get { return actionList.elapsedTime; }
        }

        Object ITaskSystem.contextObject {
            get { return this; }
        }

        public static ActionListPlayer Create() {
            return new GameObject("ActionList").AddComponent<ActionListPlayer>();
        }

        public void SendTaskOwnerDefaults() {
            actionList.SetOwnerSystem(this);
            foreach ( var a in actionList.actions ) {
                a.SetOwnerSystem(this);
            }
        }

        void ITaskSystem.SendEvent(ParadoxNotion.EventData eventData, object sender) {
            Debug.LogWarning("Sending events to action lists has no effect");
        }

        void ITaskSystem.RecordUndo(string name) {
#if UNITY_EDITOR
            if ( !Application.isPlaying ) {
                UnityEditor.Undo.RecordObject(this, name);
            }
#endif
        }

        void Awake() {
            SendTaskOwnerDefaults();
        }

        [Button]
        public void Play() {
            Play(this, this.blackboard, null);
        }

        public void Play(System.Action<bool> OnFinish) {
            Play(this, this.blackboard, OnFinish);
        }

        public void Play(Component agent, IBlackboard blackboard, System.Action<bool> OnFinish) {
            if ( Application.isPlaying ) {
                actionList.ExecuteAction(agent, blackboard, OnFinish);
            }
        }

        public Status ExecuteAction() {
            return actionList.ExecuteAction(this, blackboard);
        }

        public Status ExecuteAction(Component agent) {
            return actionList.ExecuteAction(agent, blackboard);
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        void Reset() {
            var bb = GetComponent<Blackboard>();
            _blackboard = bb != null ? bb : gameObject.AddComponent<Blackboard>();
            _actionList = (ActionList)NodeCanvas.Framework.Task.Create(typeof(ActionList), this);
        }

        void OnValidate() {
            if ( !Application.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                SendTaskOwnerDefaults();
            }
        }

#endif
	}
}