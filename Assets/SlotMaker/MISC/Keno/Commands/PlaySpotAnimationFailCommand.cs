using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New PlaySpotAnimationPickFail", menuName = "SlotMaker/Keno/Commands/Spot/PlaySpotAnimationPickFail")]
    public class PlaySpotAnimationFailCommand : Command<SpotInstance, Animator>
    {
        public int layer = 0;

        protected override void OnExecute(SpotInstance spot)
        {
            agent.Play("PickFail", layer);

            EndAction();
        }
    }
}
