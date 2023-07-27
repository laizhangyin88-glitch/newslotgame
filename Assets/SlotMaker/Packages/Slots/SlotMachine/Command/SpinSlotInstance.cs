using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Spin SlotInstance", menuName="SlotMaker2/Slot/Command/SlotInstance/Spin")]
    public class SpinSlotInstance : CommandAsset
    {
        public int layer;
        public float time;
        public bool inverse;

        [Serializable]
        public class SpinSlotInstanceCommand : Command<SlotInstance, SpinSlotInstance>
        {
            protected int index;
            protected float time;

            protected override void OnExecute(float deltaTime)
            {
                if (sharedCommand.time < deltaTime)
                {
                    var reels = agent.layers[sharedCommand.layer].reels;
                    for (int i = 0, count = reels.Count; i < count; ++i)
                    {
                        var reel = reels[!sharedCommand.inverse ? i : (count - i - 1)];
                        if (!reel.locked.value) reel.Spin();
                    }
                    EndCommand();
                }
                else
                {
                    Debug.Log(deltaTime);
                    index = 0;
                    SpinNext();
                }
            }

            protected override void OnUpdate(float deltaTime)
            {
                time += deltaTime;
                if (time >= sharedCommand.time)
                    SpinNext();
            }

            protected override void OnSkip()
            {
                if (sharedCommand.skippable)
                {
                    while (status == Status.Running)
                        SpinNext();
                }
            }

            protected void SpinNext()
            {
                var reels = agent.layers[sharedCommand.layer].reels;
                int found = 0;
                for (int count = reels.Count; index < count; ++index)
                {
                    int i = !sharedCommand.inverse ? index : (count - index - 1);
                    var reel = reels[i];
                    if (!reel.locked.value)
                    {
                        ++found;
                        if (found == 1)
                        {
                            reel.Spin();
                        }
                        else if (found > 1)
                        {
                            time = 0f;
                            break;
                        }
                    } 
                }
                if (found < 2)
                    EndCommand();
            }
        }

        public override Command Create()
        {
            return new SpinSlotInstanceCommand {
                sharedCommand = this
            };
        }
    }
}