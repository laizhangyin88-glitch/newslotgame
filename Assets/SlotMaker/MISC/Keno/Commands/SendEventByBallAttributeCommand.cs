using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New SendEventByBallAttribute", menuName = "SlotMaker/Keno/Commands/Ball/SendEventByBallAttribute")]
    public class SendEventByBallAttributeCommand : Command<BallInstance, BallInstance>
{
        public SymbolAttribute ballAttribute;
        public String eventName;

        protected override void OnExecute(BallInstance ball)
        {
            if (SymbolMask.HasAttribute(ball.mask, ballAttribute))
                agent.SendEvent(eventName);

            EndAction();
        }
    }
}
