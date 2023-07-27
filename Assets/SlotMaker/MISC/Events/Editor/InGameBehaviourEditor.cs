using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(InGameBehaviour))]
    public class InGameBehaviourEditor : Editor
    {
        private InGameBehaviour behaviour;

        private void OnEnable()
        {
            behaviour = target as InGameBehaviour;
        }

        public override void OnInspectorGUI()
        {
            behaviour.showEvents = EditorGUILayout.Foldout(behaviour.showEvents, behaviour.showEvents ? "Hide Events" : "Show Events");
            if (behaviour.showEvents)
            {
                behaviour.enableSystemEvent = EditorGUILayout.Toggle("Enable System Event", behaviour.enableSystemEvent);
                behaviour.enableContentEvent = EditorGUILayout.Toggle("Enable Content Event", behaviour.enableContentEvent);
                behaviour.enableSpinButtonEvent = EditorGUILayout.Toggle("Enable SpinButton Event", behaviour.enableSpinButtonEvent);

                serializedObject.Update();
                if (behaviour.enableSystemEvent)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onSystemReset"), false);
                }
                if (behaviour.enableContentEvent)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnterGame"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onExitGame"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onLeaveGame"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onInitGame"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onReadyGame"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnterTurn"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onExitTurn"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onFailSpin"), false);
                }
                if (behaviour.enableSpinButtonEvent)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("onSpinButton"), false);
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}
