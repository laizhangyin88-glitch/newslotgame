using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameLevelLocked : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> iconAreaElement;
        public BBParameter<ContextElement> badgeAreaElement;
        public BBParameter<ContextElement> lockedIconElement;
        public BBParameter<ContextElement> lockedSpeechBalloonElement;
        public BBParameter<bool> ignoreSpeechBalloon;

        protected override void OnExecute()
        {
            iconAreaElement.value = ContextUtils.FindElement(agent, "Icon Area", ContextSearchingType.ChildrenSearch);
            badgeAreaElement.value = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement.value = ContextUtils.FindElement(agent, "Locked Area/Collecting Game Icon Locked", ContextSearchingType.FullNameSearch);
            lockedSpeechBalloonElement.value = ContextUtils.FindElement(agent, "Locked Area/Collecting Game Locked Info Speech Balloon", ContextSearchingType.FullNameSearch);

            int level = MetaGameUtils.GetMetaUnlockedLevel();
            MetaContextElementUtils.SimpleSetText(lockedIconElement.value, "Text", level.ToString());

            if (!ignoreSpeechBalloon.value)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement.value, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", ContextSearchingType.ChildrenSearch, level);
            }

            EndAction();
        }
    }
}