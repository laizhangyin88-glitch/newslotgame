using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New CatchSpot", menuName = "SlotMaker/Keno/Commands/Ball/CatchSpot")]
    public class CatchSpotCommand : Command<BallInstance, SpotInstance>
    {
        protected override void OnExecute(BallInstance ball)
        {
            if (ball.number == agent.number)
                agent.Catch();

            EndAction();
        }
    }
}
