using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New HitBallFromSpot", menuName = "SlotMaker/Keno/Commands/Spot/HitBallFromSpot")]
    public class HitBallFromSpotCommand : Command<SpotInstance, BallInstance>
    {
        protected override void OnExecute(SpotInstance spot)
        {
            if (spot.IsHit && spot.number == agent.number)
                agent.SendEvent("Hit");

            EndAction();
        }
    }
}
