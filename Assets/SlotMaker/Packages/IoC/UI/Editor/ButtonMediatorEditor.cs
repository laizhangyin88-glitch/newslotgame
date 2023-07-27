using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace SlotMaker.IoC.UI.Editor
{
    [CustomEditor(typeof(ButtonMediator))]
    public class ButtonMediatorEditor : ButtonEditor
    {
        private SerializedProperty scaleTargetProperty;
        private SerializedProperty tweenProperty;
        private SerializedProperty baseScaleProperty;
        private SerializedProperty pressedScaleProperty;
        private SerializedProperty timeControlProperty;
        private SerializedProperty onResetProperty;
        private SerializedProperty onNormalProperty;
        private SerializedProperty onHighlightedProperty;
        private SerializedProperty onPressedProperty;
        private SerializedProperty onEnabledProperty;
        private SerializedProperty onDisabledProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            scaleTargetProperty = serializedObject.FindProperty("scaleTarget");
            tweenProperty = serializedObject.FindProperty("tween");
            baseScaleProperty = serializedObject.FindProperty("baseScale");
            pressedScaleProperty = serializedObject.FindProperty("pressedScale");
            timeControlProperty = serializedObject.FindProperty("timeControl");
            onResetProperty = serializedObject.FindProperty("onReset");
            onNormalProperty = serializedObject.FindProperty("onNormal");
            onHighlightedProperty = serializedObject.FindProperty("onHighlighted");
            onPressedProperty = serializedObject.FindProperty("onPressed");
            onEnabledProperty = serializedObject.FindProperty("onEnabled");
            onDisabledProperty = serializedObject.FindProperty("onDisabled");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(scaleTargetProperty);
            EditorGUILayout.PropertyField(tweenProperty);
            EditorGUILayout.PropertyField(baseScaleProperty);
            EditorGUILayout.PropertyField(pressedScaleProperty);
            EditorGUILayout.PropertyField(timeControlProperty);
            EditorGUILayout.PropertyField(onResetProperty);
            EditorGUILayout.PropertyField(onNormalProperty);
            EditorGUILayout.PropertyField(onHighlightedProperty);
            EditorGUILayout.PropertyField(onPressedProperty);
            EditorGUILayout.PropertyField(onEnabledProperty);
            EditorGUILayout.PropertyField(onDisabledProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}