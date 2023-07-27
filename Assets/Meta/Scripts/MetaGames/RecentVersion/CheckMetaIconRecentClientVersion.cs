using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class CheckMetaIconRecentClientVersion : ActionTask
    {
        public BBParameter<bool> saveAsRecentIcon;

        protected override string info
        {
            get { return "Check Meta Icon Recent Update"; }
        }

        protected override void OnExecute()
        {
            // Admin setting - Kill switch
            Variable<bool> enableNeedUpdate = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_NEED_UPDATE_META_ICON");
            if (enableNeedUpdate == null || enableNeedUpdate.value == false)
            {
                saveAsRecentIcon.value = false;
                EndAction();
                return;
            }

            int clientVersion = ApplicationSettings.GetClientVersionNumber();
            int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;

            saveAsRecentIcon.value = clientVersion < recentVersion;
            EndAction();
        }
    }
}