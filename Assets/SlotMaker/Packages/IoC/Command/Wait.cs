using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName="New Wait", menuName="SlotMaker2/Slot/Command/Wait")]
    public class Wait : CommandAsset
    {
        public float time;

        [Serializable]
        public class WaitCommand : Command<Component, Wait>
        {
            private float time;

            protected override void OnExecute(float deltaTime)
            {
                if (sharedCommand.time < deltaTime)
                    EndCommand();
                else
                    time = 0f;
            }

            protected override void OnUpdate(float deltaTime)
            {
                time += deltaTime;
                if (time >= sharedCommand.time)
                    EndCommand();
            }
        }

        public override Command Create()
        {
            return new WaitCommand {
                sharedCommand = this
            };
        }
    }
}