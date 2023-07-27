using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.LuckyFive.Tasks.Actions
{
    [Category("★ BagelCode/LuckyFive")]
    public class UpdateLuckyFiveLevelLocked : ActionTask // todo remove
    {
        public BBParameter<ContextElement> iconAreaElement;
        public BBParameter<ContextElement> lockedIconElement;
        public BBParameter<ContextElement> lockedSpeechBalloonElement;

        protected override void OnExecute()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (metaGameEnterInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

                iconAreaElement.value?.gameObject.SetActive(!isLockedLevel);

                lockedIconElement.value?.gameObject.SetActive(isLockedLevel);
                lockedSpeechBalloonElement.value?.gameObject.SetActive(isLockedLevel);
            }

            EndAction();
        }
    }
}
