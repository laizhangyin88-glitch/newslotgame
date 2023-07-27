using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine.CrashReportHandler;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class SetSceneState : ActionTask<Transform>
    {
        public BBParameter<SceneState> sceneState;

        protected override string info
        {
            get
            {
                return string.Format("Set Scene State : {0}", sceneState);
            }
        }

        protected override void OnExecute()
        {
            var currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "currentSceneState");
            var prevSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "prevSceneState");

            CrashReportHandler.SetUserMetadata("Meta.prevSceneState", prevSceneState.value.ToString());
            CrashReportHandler.SetUserMetadata("Meta.currentSceneState", currentSceneState.value.ToString());

            prevSceneState.value = currentSceneState.value;
            currentSceneState.value = sceneState.value;

            EndAction();
        }
    }
}
