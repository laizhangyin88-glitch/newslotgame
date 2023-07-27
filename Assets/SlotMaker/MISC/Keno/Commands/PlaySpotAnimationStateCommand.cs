using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New PlaySpotAnimationState", menuName = "SlotMaker/Keno/Commands/Spot/PlaySpotAnimationState")]
    public class PlaySpotAnimationStateCommand : Command<SpotInstance, Animator>
    {
        public int layer = 0;

        protected override void OnExecute(SpotInstance spot)
        {
            if (spot.catchState == CatchState.Catch)
            {
                if (spot.markState == MarkState.Mark)
                    agent.Play("Hit", layer);
                else
                    agent.Play("Miss", layer);
            }
            else
            {
                if (spot.markState == MarkState.Mark)
                    agent.Play("Mark", layer);
                else
                    agent.Play("Unmark", layer);
            }

            EndAction();
        }
    }
}
