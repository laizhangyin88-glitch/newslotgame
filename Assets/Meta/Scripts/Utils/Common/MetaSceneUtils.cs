using SlotMaker;
using UnityEngine.CrashReportHandler;

namespace BagelCode
{
    public static class MetaSceneUtils
    {
        public static void SetSceneState(SceneState state)
        {
            var currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>("/currentSceneState");
            var prevSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>("/prevSceneState");

            CrashReportHandler.SetUserMetadata("Meta.prevSceneState", prevSceneState.value.ToString());
            CrashReportHandler.SetUserMetadata("Meta.currentSceneState", currentSceneState.value.ToString());

            prevSceneState.value = currentSceneState.value;
            currentSceneState.value = state;
        }
    }
}
