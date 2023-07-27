using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace BagelCode
{
    [CustomEditor( typeof(PlatformInputField))]
    public class PlatformInputFieldHelper : InputFieldEditor
    {
        private SerializedProperty onEndInputProp;
        private SerializedProperty isSequenceChatProp;
        private SerializedProperty isClearEndEditProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            onEndInputProp = serializedObject.FindProperty("onEndInput");
            isSequenceChatProp = serializedObject.FindProperty("isSequenceChat");
            isClearEndEditProp = serializedObject.FindProperty("isClearEndEdit");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(onEndInputProp);
            EditorGUILayout.PropertyField(isSequenceChatProp);
            EditorGUILayout.PropertyField(isClearEndEditProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}