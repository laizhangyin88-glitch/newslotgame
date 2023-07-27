using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameSound : ActionTask<ContextElement>
    {
        public BBParameter<string> bundle;

        private const string soundPrefabName = "Collecting Game Sounds";

        protected override void OnExecute()
        {
            if (agent != null)
            {
                var customSoundData = agent.gameObject.GetComponentInChildren<MetaCustomSoundData>();
                if (customSoundData == null)
                    MetaObjectUtils.MakePrefab(bundle.value, soundPrefabName, agent.transform);
            }
            EndAction();
        }
    }
}