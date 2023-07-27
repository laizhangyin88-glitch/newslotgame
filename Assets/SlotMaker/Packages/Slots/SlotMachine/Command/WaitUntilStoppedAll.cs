using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Wait Until Stopped All", menuName="SlotMaker2/Slot/Command/SlotInstance/Wait Until Stopped All")]
    public class WaitUntilStoppedAll : CommandAsset
    {
        public int layer;

        [Serializable]
        public class WaitUntilStoppedAllCommand : Command<SlotInstance, WaitUntilStoppedAll>
        {
            protected override void OnUpdate(float deltaTime)
            {
                var reels = agent.layers[sharedCommand.layer].reels;
                foreach (var reel in reels)
                {
                    if (reel.spinState != SpinState.Stopped)
                        return;
                }
                EndCommand();
            }
        }

        public override Command Create()
        {
            return new WaitUntilStoppedAllCommand {
                sharedCommand = this
            };
        }
    }
}
