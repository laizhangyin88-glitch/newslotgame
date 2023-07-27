using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class BgmSoundView : MonoBehaviour
    {
        public MessageRouter router;

        public List<GameSound> sounds;
        public List<int> bonusIds;

        public bool isNullOnNotSpecifiedBonus = true;

        private GameSound currentSound = null;
        public GameSound lastBgmSound { get { return currentSound; } }
        public GameSound bgmSound
        {
            get
            {
                currentSound = GetCurrentSound();
                return currentSound;
            }
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

        private GameSound GetBonusSound(int bonusId)
        {
            int index = FindBonus(bonusId);
            var notSpecifiedBonusSound = (isNullOnNotSpecifiedBonus) ? null : lastBgmSound;
            return (index >= 0) ? sounds[index + 1] : notSpecifiedBonusSound;
        }

        private GameSound GetCurrentSound()
        {
            var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus");
            if (bonus == null)
                return sounds[0];

            int bonusId = bonus.value.GetValue<int>("bonusId");
            return GetBonusSound(bonusId);
        }
    }
}
