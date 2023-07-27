using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class ReelEventForwarder : MonoBehaviour
    {
        private const string prepareStopAnimationName = "PrepareStop";
        private const string stopEffectAnimationName = "Stop Effect";

        public void OnPrepareStoppedReel(BaseReel reel)
        {
            reel.Visit((sb) => { sb.Play(prepareStopAnimationName); });
        }

        public void OnPrepareStoppedSpecialReel(BaseReel reel)
        {
            var spots = ContentCustomData.GetSlotData(reel.slotMachine.slotIndex).expectation.expectationSpots[reel.reelIndex];
            if (spots.Count > 0)
            {
                reel.Play(stopEffectAnimationName);
            }
        }
    }
}
