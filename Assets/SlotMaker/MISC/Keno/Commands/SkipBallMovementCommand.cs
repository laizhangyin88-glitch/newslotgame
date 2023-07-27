using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New SkipBallMovement", menuName = "SlotMaker/Keno/Commands/Ball/SkipBallMovement")]
    public class SkipBallMovementCommand : Command<BallInstance, Rigidbody>
    {
        protected override void OnExecute(BallInstance ball)
        {
            ball.transform.GetComponent<RectTransform>().anchoredPosition3D = ball.target;
            agent.isKinematic = true;

            EndAction();
        }
    }
}
