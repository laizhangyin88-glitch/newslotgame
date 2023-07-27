using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New ChangeSpinState", menuName="SlotMaker2/Slot/Command/ChangeSpinState")]
    public class ChangeSpinState : CommandAsset
    {
        public SpinState spinState;

        [Serializable]
        public class ChangeSpinStateCommand : Command<ISpinnable, ChangeSpinState>
        {
            protected override void OnExecute(float deltaTime)
            {
                agent.spinState = sharedCommand.spinState;
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new ChangeSpinStateCommand {
                sharedCommand = this
            };
        }
    }
}