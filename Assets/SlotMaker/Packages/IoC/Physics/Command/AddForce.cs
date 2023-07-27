using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName="New AddForce", menuName="SlotMaker2/Slot/Command/AddForce")]
    public class AddForce : CommandAsset
    {
        public Vector3 force;
        public ForceMode forceMode;
        public float time;

        [Serializable]
        public class AddForceCommand : Command<IBody, AddForce>
        {
            private float time;

            protected override void OnExecute(float deltaTime)
            {
                if (sharedCommand.time < deltaTime)
                {
                    agent.AddForce(sharedCommand.force, sharedCommand.forceMode);
                    EndCommand();
                }
                else
                {
                    time = 0f;
                }
            }

            protected override void OnUpdate(float deltaTime)
            {
                agent.AddForce(sharedCommand.force, sharedCommand.forceMode);
                
                time += deltaTime;
                if (time >= sharedCommand.time)
                    EndCommand();
            }
        }

        public override Command Create()
        {
            return new AddForceCommand {
                sharedCommand = this
            };
        }
    }
}