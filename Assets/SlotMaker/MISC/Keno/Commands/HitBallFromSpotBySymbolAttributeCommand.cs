using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New HitBallFromSpotBySymbolAttribute", menuName = "SlotMaker/Keno/Commands/Spot/HitBallFromSpotBySymbolAttribute")]
    public class HitBallFromSpotBySymbolAttributeCommand : Command<SpotInstance, BallInstance>
    {
        public SymbolAttribute mask;
        public string eventName;
        protected override void OnExecute(SpotInstance spot)
        {
            if (SymbolMask.HasAttribute(spot.mask, mask) && spot.number == agent.number)
                agent.SendEvent(eventName);

            EndAction();
        }
    }
}
