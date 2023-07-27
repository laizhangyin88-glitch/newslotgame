using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextText))]
    [CanEditMultipleObjects]
    public class ContextTextEditor : Editor
    {
        ContextText scripts;

        private void OnEnable()
        {
            scripts = (ContextText)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<UnityEngine.UI.Text>();
            if (component != null) scripts.text = component;
        }
    }
}
