using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGameLevelLocked : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> iconAreaElement;
        public BBParameter<ContextElement> badgeAreaElement;
        public BBParameter<ContextElement> lockedIconElement;
        public BBParameter<ContextElement> lockedSpeechBalloonElement;
        public BBParameter<bool> ignoreSpeechBalloon;

        protected override void OnExecute()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (metaGameEnterInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

                iconAreaElement.value.gameObject.SetActive(!isLockedLevel);
                badgeAreaElement.value.gameObject.SetActive(!isLockedLevel);

                lockedIconElement.value.gameObject.SetActive(isLockedLevel);

                if (!ignoreSpeechBalloon.value)
                {
                    lockedSpeechBalloonElement.value.gameObject.SetActive(isLockedLevel);
                }
            }
            else
            {
                Debug.LogWarning(string.Format(
                    "UpdateCollectingGameLevelLocked failure. " +
                    "eventInfo != null:{0}, isLockedFeature:{1}, IsMetaEventLevelLock:{2}",
                    metaGameEnterInfo != null, isLockedFeature, MetaGameUtils.IsMetaEventLevelLock()));
            }

            EndAction();
        }
    }
}
