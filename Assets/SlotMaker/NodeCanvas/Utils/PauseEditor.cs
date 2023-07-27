using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ Utility")]
    [Description("Set Pause in editor (like breakpoint)")]
    public class PauseEditor : ActionTask
    {
        public BBParameter<bool> disable = false;

        protected override void OnExecute()
        {
#if UNITY_EDITOR
            if (disable.value)
            {
                EndAction();
                return;
            }
            UnityEditor.EditorApplication.isPaused = true;
            UnityEditor.EditorApplication.pauseStateChanged += OnPauseStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            EndAction();
#endif
        }

#if UNITY_EDITOR
        private void UnSubscribe()
        {
            UnityEditor.EditorApplication.pauseStateChanged -= OnPauseStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange obj)
        {
            UnSubscribe();
            EndAction();
        }

        private void OnPauseStateChanged(UnityEditor.PauseState obj)
        {
            UnSubscribe();
            EndAction();
        }
#endif
    }
}
