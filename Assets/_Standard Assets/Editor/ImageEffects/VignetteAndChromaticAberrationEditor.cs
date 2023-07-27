using System;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [CustomEditor (typeof(VignetteAndChromaticAberration))]
    class VignetteAndChromaticAberrationEditor : Editor
    {
        private SerializedObject m_SerObj;
        private SerializedProperty m_Intensity;             // intensity == 0 disables pre pass (optimization)


        void OnEnable ()
        {
            m_SerObj = new SerializedObject (target);
            m_Intensity = m_SerObj.FindProperty ("intensity");
        }


        public override void OnInspectorGUI ()
        {
            m_SerObj.Update ();

            EditorGUILayout.LabelField("Simulates the common lens artifacts 'Vignette' and 'Aberration'", EditorStyles.miniLabel);

            EditorGUILayout.Slider(m_Intensity, 0.0f, 1.0f, new GUIContent("Vignetting"));

            m_SerObj.ApplyModifiedProperties();
        }
    }
}
