using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_UpdateMultiplier : ActionTask 
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<int> handsCount;
        public BBParameter<List<long>> multipliers;

        public bool updateCurrentMultiplier;

        protected override string info
        {
            get { return  updateCurrentMultiplier ? "VideoPoker_UpdateCurrentMultiplier" : "VideoPoker_UpdateNextMultiplier"; }
        }

        protected override void OnExecute()
        {
            var vp = videoPoker.value.GetComponent<VideoPoker>();

            for (int hand = 0; hand < handsCount.value; ++hand)
            {
                long multiplier = multipliers.value[hand];
                if (updateCurrentMultiplier)
                {
                    if (multiplier > 1L)
                        vp.ShowCurrMultiplier(hand, multiplier);
                    else
                        vp.HideCurrMultiplier(hand);
                }
                else
                {
                    if (multiplier > 1L)
                        vp.ShowNextMultiplier(hand, multiplier);
                    else
                        vp.HideNextMultiplier(hand);
                }
            }

            EndAction();
        }
    }
}