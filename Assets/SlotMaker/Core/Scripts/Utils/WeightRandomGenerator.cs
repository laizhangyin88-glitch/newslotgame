using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    public abstract class WeightRandomGeneratorBase : MonoBehaviour
    {
        public abstract int TakeOne();
    }
    public abstract class WeightRandomGeneratorBase<T> : WeightRandomGeneratorBase
    {
        public List<T> probabilities;
    }

    public class WeightRandomGenerator : WeightRandomGeneratorBase<float>
    {
        public override int TakeOne()
        {
            float acc = 0f;
            float rnd = UnityEngine.Random.value;
            for (int i = 0; i < (probabilities.Count - 1); ++i)
            {
                acc += probabilities[i];
                if (rnd < acc)
                    return i;
            }

            return probabilities.Count - 1;
        }
    }
}