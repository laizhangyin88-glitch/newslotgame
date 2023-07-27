using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class MetaGameCrashReport : ActionTask
    {
        public BBParameter<bool> isOtherMetaGame;
        public BBParameter<string> saveAsName;

        protected override void OnExecute()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (metaGameEnterInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                saveAsName.value = !isOtherMetaGame.value ? BlackboardQueryUtils.GetMetaGameEventName() : BlackboardQueryUtils.GetOtherMetaGameEventName();
                BlackboardQueryUtils.MetaGameCrashReport(saveAsName.value);
            }
            EndAction();
        }
    }
}