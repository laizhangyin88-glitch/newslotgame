using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace SlotMaker
{
    [CustomEditor(typeof(RayMaskableImage), true)]
    [CanEditMultipleObjects]
    public class RayMaskableImageEditor : ImageEditor
    {
        SerializedProperty shader;
        SerializedProperty rayMaskTarget;
        SerializedProperty maskSize;
        SerializedProperty rayMask;
        SerializedProperty maskTex;

        protected override void OnEnable()
        {
            base.OnEnable();

            shader        = serializedObject.FindProperty("_shader");
            rayMaskTarget = serializedObject.FindProperty("_rayMaskTarget");
            maskSize      = serializedObject.FindProperty("_maskSize");
            rayMask       = serializedObject.FindProperty("_rayMask");
            maskTex       = serializedObject.FindProperty("_maskTex");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(shader);
            EditorGUILayout.PropertyField(rayMaskTarget);
            EditorGUILayout.PropertyField(maskSize);
            EditorGUILayout.PropertyField(rayMask);
            EditorGUILayout.PropertyField(maskTex);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
