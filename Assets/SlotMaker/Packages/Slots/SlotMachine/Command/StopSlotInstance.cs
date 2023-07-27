using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Stop SlotInstance", menuName="SlotMaker2/Slot/Command/SlotInstance/Stop")]
    public class StopSlotInstance : CommandAsset
    {
        public int layer;
        public SpinState constraintState = SpinState.PrepareStopped;
        public SpinState lastState = SpinState.PrepareStopped;
        public List<float> times;
        public SpinOutputExpectationSubset expectationSubset;
        public bool expectationSkippable;
        public bool inverse;

        [Serializable]
        public class StopSlotInstanceCommand : Command<SlotInstance, StopSlotInstance>
        {
            protected ReelInstance prev;
            protected int index;
            protected int prevIndex;
            protected int stopCount;
            protected List<bool> expectation;
            protected bool hasNext;

            protected override void OnExecute(float deltaTime)
            {
                FirstRun();
            }

            private void FirstRun()
            {
                prev = null;
                index = prevIndex = stopCount = 0;
                expectation = sharedCommand.expectationSubset.GetExpectedReels();
                if (expectation.Count == 0)
                    expectation = null;

                StopNext();
            }

            protected override void OnUpdate(float deltaTime)
            {
                while (hasNext)
                {
                    if (((int)prev.spinState >= (int)sharedCommand.constraintState) &&
                        (Time.time >= prev.GetSpinStateTime(sharedCommand.constraintState) + sharedCommand.times[stopCount - 1]))
                    {
                        StopNext();
                    }
                    else
                    {
                        break;
                    }
                }

                if (!hasNext && ((int)prev.spinState >= (int)sharedCommand.lastState))
                {
                    if (expectation != null && expectation[prevIndex])
                        sharedCommand.expectationSubset.EndExpectation(prevIndex);

                    EndCommand();
                }
            }

            protected override void OnSkip()
            {
                if (sharedCommand.skippable)
                {
                    if (status == Status.Resting)
                    {
                        status = Status.Running;
                        FirstRun();
                    }

                    if (hasNext)
                    {
                        while (hasNext)
                        {
                            if (StopNext(true))
                                break;
                        }
                    }
                    else if (prev)
                    {
                        prev.Skip();
                    }
                }
            }

            protected bool StopNext(bool skip = false)
            {
                var reels = agent.layers[sharedCommand.layer].reels;
                int found = 0;
                bool isExpectationReel = false;
                for (int count = reels.Count; index < count; ++index)
                {
                    int i = !sharedCommand.inverse ? index : (count - index - 1);
                    var reel = reels[i];
                    if (!reel.locked.value)
                    {
                        ++found;
                        if (found == 1)
                        {
                            if (prev != null)
                            {
                                if (expectation != null && expectation[prevIndex])
                                    sharedCommand.expectationSubset.EndExpectation(prevIndex);
                                    
                                if (skip) prev.Skip();
                            }
                            
                            if (expectation != null && expectation[i])
                            {
                                if (!(skip && sharedCommand.expectationSkippable))
                                {
                                    sharedCommand.expectationSubset.BeginExpectation(i);
                                    reel.Expectation();
                                    isExpectationReel = true;
                                }
                            }

                            reel.Stop();
                            prev = reel;
                            prevIndex = i;
                            ++stopCount;
                        }
                        else if (found > 1)
                            break;
                    } 
                }
                
                hasNext = found > 1;
                
                return isExpectationReel;
            }
        }

        public override Command Create()
        {
            return new StopSlotInstanceCommand {
                sharedCommand = this
            };
        }
    }
}