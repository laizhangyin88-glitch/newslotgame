using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextTextMeshProUGUI))]
    [CanEditMultipleObjects]
    public class ContextTextMeshProUGUIEditor : Editor
    {
        ContextTextMeshProUGUI scripts;

        private void OnEnable()
        {
            scripts = (ContextTextMeshProUGUI)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<TMPro.TextMeshProUGUI>();
            if (component != null) scripts.textMeshProUGUI = component;
        }
    }
}
