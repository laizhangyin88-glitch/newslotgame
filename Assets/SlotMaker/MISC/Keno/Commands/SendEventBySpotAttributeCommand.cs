using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno.Commands
{
    [CreateAssetMenu(fileName = "New SendEventBySpotAttribute", menuName = "SlotMaker/Keno/Commands/Spot/SendEventBySpotAttribute")]
    public class SendEventBySpotAttributeCommand : Command<SpotInstance, SpotInstance>
{
        public SymbolAttribute spotAttribute;
        public String eventName;

        protected override void OnExecute(SpotInstance spot)
        {
            if (SymbolMask.HasAttribute(spot.mask, spotAttribute))
                agent.SendEvent(eventName);

            EndAction();
        }
    }
}
