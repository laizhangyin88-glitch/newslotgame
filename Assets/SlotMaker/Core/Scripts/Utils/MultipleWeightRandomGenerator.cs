using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [System.Serializable]
    public class ProbabilityInfo
    {
        public float[] probabilities;
    }
    public class MultipleWeightRandomGenerator : WeightRandomGeneratorBase<ProbabilityInfo>
    {
        public int currentWeightsIndex = 0;
        public override int TakeOne()
        {
            var actualProbabilities = probabilities[currentWeightsIndex].probabilities;
            float acc = 0f;

            float rnd = UnityEngine.Random.value;
            for (int i = 0; i < (actualProbabilities.Length - 1); ++i)
            {
                acc += actualProbabilities[i];
                if (rnd < acc)
                    return i;
            }

            return actualProbabilities.Length - 1;
        }
    }
}
