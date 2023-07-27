using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New SetDampingAndDrag", menuName = "SlotMaker2/Slot/Command/SetDampingAndDrag")]
    public class SetDampingAndDrag : CommandAsset
    {
        public float damping;
        public float drag;

        [Serializable]
        public class SetDampingAndDragCommand : Command<Reel2D, SetDampingAndDrag>
        {
            protected override void OnExecute(float deltaTime)
            {
                agent.damping = sharedCommand.damping;
                agent.drag = sharedCommand.drag;
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new SetDampingAndDragCommand
            {
                sharedCommand = this
            };
        }
    }
}