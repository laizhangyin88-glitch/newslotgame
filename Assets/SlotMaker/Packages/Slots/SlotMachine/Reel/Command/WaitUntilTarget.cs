using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New WaitUntil Target", menuName="SlotMaker2/Slot/Command/WaitUntilTarget")]
    public class WaitUntilTarget : CommandAsset
    {
        public RectTransform.Axis axis = RectTransform.Axis.Vertical;
        public CompareMethod compareMethod;
        public float epsilon = Mathf.Epsilon;

        [Serializable]
        public class WaitUntilTargetCommand : Command<Reel2D, WaitUntilTarget>
        {
            protected override void OnLateUpdate(float deltaTime)
            {
                switch (sharedCommand.axis)
                {
                case RectTransform.Axis.Horizontal:
                    if (OperationUtils.Compare(agent.position.x, agent.desiredPosition.x, sharedCommand.compareMethod, sharedCommand.epsilon))
                        FinalizeCommand();
                    break;
                case RectTransform.Axis.Vertical:
                    if (OperationUtils.Compare(agent.position.y, agent.desiredPosition.y, sharedCommand.compareMethod, sharedCommand.epsilon))
                        FinalizeCommand();
                    break;
                }
            }

            private void FinalizeCommand()
            {
                agent.position = agent.desiredPosition;
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new WaitUntilTargetCommand {
                sharedCommand = this
            };
        }
    }
}