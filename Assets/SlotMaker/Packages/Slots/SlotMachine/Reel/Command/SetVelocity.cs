using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New SetVelocity", menuName = "SlotMaker2/Slot/Command/SetVelocity")]
    public class SetVelocity : CommandAsset
    {
        public Vector3 velocity;

        [Serializable]
        public class SetVelocityCommand : Command<Reel2D, SetVelocity>
        {
            protected override void OnExecute(float deltaTime)
            {
                agent.SetVelocity(sharedCommand.velocity);
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new SetVelocityCommand
            {
                sharedCommand = this
            };
        }
    }
}