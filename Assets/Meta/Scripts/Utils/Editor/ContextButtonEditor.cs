using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextButton))]
    [CanEditMultipleObjects]
    public class ContextButtonEditor : Editor
    {
        ContextButton scripts;

        private void OnEnable()
        {
            scripts = (ContextButton)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<UnityEngine.UI.Button>();
            if (component != null) scripts.button = component;
        }
    }
}
