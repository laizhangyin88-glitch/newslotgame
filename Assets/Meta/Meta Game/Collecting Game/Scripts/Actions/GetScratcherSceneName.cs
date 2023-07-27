using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class GetScratcherSceneName : ActionTask
    {
        public BBParameter<Blackboard> scratcherRewardResult;
        public BBParameter<string> sceneName;
        
        protected override string info
        {
            get { return "Get Scratcher Scene Name"; }
        }

        protected override void OnExecute()
        {
            ScratcherName name = scratcherRewardResult.value.GetValue<ScratcherName>("scratcherName");

            sceneName.value = PopupScratcherUtils.GetScratcherSceneName(name);

            if (sceneName.value == string.Empty)
                EndAction(false);

            EndAction();
        }
    }
}