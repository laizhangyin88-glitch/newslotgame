using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextImage))]
    [CanEditMultipleObjects]
    public class ContextImageEditor : Editor
    {
        ContextImage scripts;

        private void OnEnable()
        {
            scripts = (ContextImage)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<UnityEngine.UI.Image>();
            if(component != null) scripts.image = component;
        }
    }
}
