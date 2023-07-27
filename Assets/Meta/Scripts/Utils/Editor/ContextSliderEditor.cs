using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextSlider))]
    [CanEditMultipleObjects]
    public class ContextSliderEditor : Editor
    {
        ContextSlider scripts;

        private void OnEnable()
        {
            scripts = (ContextSlider)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<UnityEngine.UI.Slider>();
            if (component != null) scripts.slider = component;
        }
    }
}
