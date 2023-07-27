using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;

namespace SlotMaker
{
    public class RandomAction : MonoBehaviour
    {
        public List<ActionListPlayer> players;
        public List<int> playerWeights;
        public float coolTimeMin;
        public float coolTimeMax;

        private bool running;

        private void OnEnable()
        {
            running = false;

            int totalWeight;
            List<int> weights = RandomUtils.GetAccumulatedWeightList(playerWeights, out totalWeight);

            StartCoroutine(RandomTrigger(weights, totalWeight));
        }

        private IEnumerator RandomTrigger(List<int> weights, int totalWeight)
        {
            while (true)
            {
                if (running)
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(Random.Range(coolTimeMin, coolTimeMax));

                int index = RandomUtils.WeightRandom(weights, totalWeight);
                running = true;
                players[index].Play(OnFinish);
            }
        }

        private void OnFinish(bool result)
        {
            running = false;
        }
    }
}
