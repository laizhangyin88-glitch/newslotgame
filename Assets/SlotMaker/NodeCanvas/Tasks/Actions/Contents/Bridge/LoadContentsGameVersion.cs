using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Contents;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Contents")]
    public class LoadContentsGameVersion : ActionTask
    {
        public BBParameter<string> gameTitle;
        protected override string info { get { return string.Format("Load the game version of {0}", gameTitle); } }

        protected override void OnExecute()
        {
            // Call GetGameVersion once to load gameInfo and cache gameVersion if it is the first enter of the game.
            ContentsVersionManager.Instance.GetGameVersion(gameTitle.value);
            EndAction();
        }
    }
}
