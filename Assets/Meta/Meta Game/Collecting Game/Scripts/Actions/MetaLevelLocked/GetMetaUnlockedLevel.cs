using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class GetMetaUnlockedLevel : ActionTask
    {
        public BBParameter<bool> saveAs;

        protected override void OnExecute()
        {
            saveAs.value = MetaGameUtils.IsMetaGameLevelLocked();
            EndAction();
        }
    }
}