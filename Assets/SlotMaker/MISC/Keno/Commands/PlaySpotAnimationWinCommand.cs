using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New PlaySpotAnimationWin", menuName = "SlotMaker/Keno/Commands/Spot/PlaySpotAnimationWin")]
    public class PlaySpotAnimationWinCommand : Command<SpotInstance, Animator>
    {
        public int layer = 0;
        public bool isSkip = false;

        protected override void OnExecute(SpotInstance spot)
        {
            if (!isSkip)
                agent.Play("Win", layer);
            else
                agent.Play("SkipWin", layer);

            EndAction();
        }
    }
}
