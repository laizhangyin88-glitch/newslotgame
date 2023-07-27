using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    [CustomEditor(typeof(ContextAnimator))]
    [CanEditMultipleObjects]
    public class ContextAnimatorEditor : Editor
    {
        ContextAnimator scripts;

        private void OnEnable()
        {
            scripts = (ContextAnimator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = scripts.GetComponent<UnityEngine.Animator>();
            if(component != null) scripts.animator = component;
        }
    }
}
