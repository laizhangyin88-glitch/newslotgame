using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.EDM.Feature
{
    public class EDMReelStripsAdmin : FeatureModule
    {
        private void Awake()
        {
            RegisterEvent("EDMUndoReelStrips", OnUndoReelStrips);
        }

        public void OnUndoReelStrips(EventData eventData)
        {
            ReelStrips strips = ((GameObject)eventData.value).GetComponent<ReelStrips>();
            for (int i = 0; i < strips.reelCount; i++)
            {
                var strip = strips.GetReelStrip(i);
                while (strip.UnDo()) ;
            }
        }
    }
}