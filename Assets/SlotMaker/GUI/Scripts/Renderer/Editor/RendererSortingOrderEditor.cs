using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(RendererSortingOrder))]
    public class RendererSortingOrderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            string[] sortingLayerNames = GetSortingLayerNames();
            int[] sortingLayerIds = GetSortingLayerUniqueIDs();
            MonoBehaviour mono = this.target as MonoBehaviour;
            Renderer renderer = mono.GetComponent<Renderer>();
            renderer.sortingLayerID = EditorGUILayout.IntPopup("Sorting Layer", renderer.sortingLayerID, sortingLayerNames, sortingLayerIds);
            renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);
        }

        public static string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, null);
        }

        public static int[] GetSortingLayerUniqueIDs()
        {
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            System.Reflection.PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, null);
        }
    }
}
