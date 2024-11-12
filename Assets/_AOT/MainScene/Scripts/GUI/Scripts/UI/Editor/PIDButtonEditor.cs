using UnityEngine.UI;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(PIDButton), true)]
    [CanEditMultipleObjects]
    public class PIDButtonEditor : UnityEditor.UI.ButtonEditor
    {
        SerializedProperty m_onInteractableChanged;
        SerializedProperty m_onDisable;
        SerializedProperty m_scaleFactor;
        SerializedProperty m_frequency;
        SerializedProperty m_damping;
        SerializedProperty m_scaleTarget;
        SerializedProperty m_disableCover;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_onInteractableChanged = serializedObject.FindProperty("onInteractableChanged");
            m_onDisable     = serializedObject.FindProperty("onDisable");
            m_scaleFactor   = serializedObject.FindProperty("scaleFactor");
            m_frequency     = serializedObject.FindProperty("frequency");
            m_damping       = serializedObject.FindProperty("damping");
            m_scaleTarget   = serializedObject.FindProperty("scaleTarget");
            m_disableCover  = serializedObject.FindProperty("disableCover");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_onInteractableChanged);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onDisable);
            EditorGUILayout.PropertyField(m_scaleFactor);
            EditorGUILayout.PropertyField(m_frequency);
            EditorGUILayout.PropertyField(m_damping);    
            EditorGUILayout.PropertyField(m_scaleTarget);
            EditorGUILayout.PropertyField(m_disableCover);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
