using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots
{
    public class ExpectedSpotsSoundController : MonoBehaviour
    {
        public SlotMediator slot;
        public List<SpinOutputExpectationSubset> outputs;

        [Serializable]
        public class ExpectationSounds
        {
            public List<string> soundList = new List<string>();

            public enum PlayMethod
            {
                UseIndex,
                UseCount
            }
            public PlayMethod playMethod = PlayMethod.UseCount;

            public void Play(int index, int count)
            {
                int soundIndex = 0;
                switch (playMethod)
                {
                case PlayMethod.UseIndex:
                    soundIndex = index;
                    break;
                case PlayMethod.UseCount:
                    soundIndex = count - 1;
                    break;
                }
                soundIndex = Mathf.Min(soundIndex, soundList.Count - 1);
                GSManager.Instance.GetHandler(soundList[soundIndex]).Play();
            }
        }
        public List<ExpectationSounds> sounds;

        public void Expectation(ReelInstance reelInstance)
        {
            var mgr = GSManager.Instance;
            if (!mgr) return;

            int index = slot.GetReelIndex(reelInstance);
            if (index >= 0)
            {
                for (int i = 0, count = outputs.Count; i < count; ++i)
                {
                    var output = outputs[i];
                    var expectedSpots = output.GetExpectedSpots();
                    if (index > (expectedSpots.Count - 1))
                        continue;

                    var spots = expectedSpots[index];
                    if (spots.Count == 0)
                        continue;

                    var expectedValues = output.GetExpectedValues();
                    sounds[i].Play(index, expectedValues[index]);
                }
            }
        }
    }
}