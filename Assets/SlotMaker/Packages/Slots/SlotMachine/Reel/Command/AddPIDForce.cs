using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New AddPIDForce", menuName="SlotMaker2/Slot/Command/AddPIDForce")]
    public class AddPIDForce : CommandAsset
    {
        public float frequency;
        public float damping;
        public float epsilon;

        [Serializable]
        public class AddPIDForceCommand : Command<Reel2D, AddPIDForce>
        {
            protected override void OnUpdate(float deltaTime)
            {
                if ((Vector3.Distance(agent.position, agent.desiredPosition) < sharedCommand.epsilon) &&
                    (Vector3.Distance(agent.velocity, Vector3.zero) < sharedCommand.epsilon))
                {
                    agent.SetVelocity(Vector3.zero);
                    EndCommand();
                    return;
                }

                float ksg, kdg;
                PIDUtils.CalcCoefficient(sharedCommand.frequency, sharedCommand.damping, deltaTime, out ksg, out kdg);
                Vector3 force = PIDUtils.CalcForce(agent.position, agent.desiredPosition, agent.velocity, Vector3.zero, ksg, kdg);
                agent.AddForce(force, ForceMode.Acceleration);
            }
        }

        public override Command Create()
        {
            return new AddPIDForceCommand {
                sharedCommand = this
            };
        }
    }
}