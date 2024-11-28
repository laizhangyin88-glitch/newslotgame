using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class RandomUtils
    {
        public static List<int> GetAccumulatedWeightList(List<int> weights, out int totalWeight)
        {
            List<int> result = new List<int>();
            int sum = 0;
            int count = weights.Count;
            for (int i = 0; i < count; ++i)
            {
                sum += weights[i];
                result.Add(sum);
            }
            totalWeight = sum;
            return result;
        }

        public static int WeightRandom(List<int> weights, int totalWeight)
        {
            int rnd = Random.Range(0, totalWeight);
            int count = weights.Count;
            for (int i = 0; i < count; ++i)
            {
                if (rnd < weights[i])
                    return i;
            }
            return count - 1;
        }

        public static List<T> WeightRandomList<T>(this List<T> samples, List<int> weights, int count)
        {
            var result = new List<T>(count);
            int totalWeight = 0;
            weights = GetAccumulatedWeightList(weights, out totalWeight);
            for (int i = 0; i < count; ++i)
            {
                int index = WeightRandom(weights, totalWeight);
                result.Add(samples[index]);
            }
            return result;
        }

        public static List<int> UniqueRandomList(int min, int max, int count)
        {
            HashSet<int> candidates = new HashSet<int>();
            while (candidates.Count < count)
            {
                candidates.Add(UnityEngine.Random.Range(min, max));
            }

            List<int> result = new List<int>();
            result.AddRange(candidates);
            return Shuffle(result);
        }

        public static List<int> UniqueRandomList(int min, int max)
        {
            int count = max - min;
            List<int> result = new List<int>(count);
            for (int i = 0; i < count; ++i)
            {
                result[i] = min + i;
            }
            return Shuffle(result);
        }

        public static List<T> Shuffle<T>(this List<T> samples)
        {
            var result = new List<T>(samples);
            int n = result.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n);
                T temp = result[k];
                result[k] = result[--n];
                result[n] = temp;
            }
            return result;
        }
    }
}
