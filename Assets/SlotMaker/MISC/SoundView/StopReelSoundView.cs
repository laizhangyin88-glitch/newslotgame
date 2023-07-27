using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    [System.Serializable]
    public class StopReelSoundSet
    {
        public List<string> reelStopSounds;
        public List<bool> ignoreReels;
    }

    public class StopReelSoundView : MonoBehaviour
    {
        public int slotIndex = 0;

        public bool isStopReelSoundMute = false;
        public bool isLastOnly = false;

        public List<StopReelSoundSet> reelStopSoundsList;
        public List<int> bonusIds;

        public static readonly string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        public static readonly string ON_PREPARE_STOPPED_REEL_EVENT = "PrepareStoppedReel";

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }
        
        private int FindBonus(int bonusId)
        {
            for (int i = 0; i < bonusIds.Count; ++i)
            {
                if (bonusId == bonusIds[i])
                    return i;
            }

            return -1;
        }

        private StopReelSoundSet GetBonusSound(int bonusId)
        {
            int index = FindBonus(bonusId);
            return (index >= 0) ? reelStopSoundsList[index + 1] : reelStopSoundsList[0];
        }

        private StopReelSoundSet GetCurrentSoundSet()
        {
            var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus");
            if (bonus == null)
                return reelStopSoundsList[0];

            int bonusId = bonus.value.GetValue<int>("bonusId");
            return GetBonusSound(bonusId);
        }

        protected virtual void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotIndex) return;
            if (receivedEvent.name.Equals(ON_PREPARE_STOPPED_REEL_EVENT, StringComparison.Ordinal))
                OnPrepareStoppedReel((int)receivedEvent.value);
        }

        protected virtual void OnPrepareStoppedReel(int reelIndex)
        {
            var currentSoundSet = GetCurrentSoundSet();
            var currentSoundsCount = currentSoundSet.reelStopSounds.Count;
            string id = currentSoundSet.reelStopSounds[ Mathf.Min(reelIndex, currentSoundsCount - 1) ];
            bool isIgnoreReel = currentSoundSet.ignoreReels[ Mathf.Min(reelIndex, currentSoundsCount - 1) ];

            if (isLastOnly)
                isIgnoreReel = (reelIndex == currentSoundsCount - 1) ? isIgnoreReel : true;

            if (!isIgnoreReel && !isStopReelSoundMute)
                GSManager.Instance.GetHandler(id).Play();
        }
    }
}
