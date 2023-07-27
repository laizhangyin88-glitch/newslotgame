using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class CustomSFXController : MonoBehaviour
    {
        public enum PickType
        {
            Random,
            RandomWithoutDuplicate,
            SequentialClamped,
            SequentialLoop,
            PingPong
        };
        public PickType pickType = PickType.SequentialClamped;

        [Serializable]
        public class SFX
        {
            public string handlerId;
            public int weight = 1;
            public ObjectPool customPool;
        }
        public List<SFX> sfxList = new List<SFX>();

        [HideInInspector]
        [SerializeField]
        private List<int> accumulatedWeights = new List<int>();

        [HideInInspector]
        [SerializeField]
        private int totalWeight;

        private int pickedIndex = -1;
        private bool reverse;

        [Button]
        public void Play()
        {
            Pick();

            if (GSManager.Instance)
                GetPickedSource().Play();
        }

        public void Reset()
        {
            pickedIndex = -1;
        }

        [Button]
        private void Pick()
        {
            switch (pickType)
            {
            case PickType.Random:
                pickedIndex = RandomUtils.WeightRandom(accumulatedWeights, totalWeight);
                break;
            case PickType.RandomWithoutDuplicate:
                {
                    int newIndex = -1;
                    do
                    {
                        newIndex = RandomUtils.WeightRandom(accumulatedWeights, totalWeight);
                    } while (newIndex == pickedIndex);
                    pickedIndex = newIndex;
                }
                break;
            case PickType.SequentialClamped:
                pickedIndex = Mathf.Clamp(++pickedIndex, 0, sfxList.Count - 1);
                break;
            case PickType.SequentialLoop:
                if (++pickedIndex > (sfxList.Count - 1))
                    pickedIndex = 0;
                break;
            case PickType.PingPong:
                if (!reverse)
                {
                    if (++pickedIndex > (sfxList.Count - 1))
                    {
                        pickedIndex = sfxList.Count - 2;
                        reverse = true;
                    }
                }
                else
                {
                    if (--pickedIndex < 0)
                    {
                        pickedIndex = 1;
                        reverse = false;
                    }
                }
                break;
            }
        }

        private GSSource GetPickedSource()
        {
            var sfx = sfxList[pickedIndex];
            var handler = GSManager.Instance.GetHandler(sfx.handlerId);
            
            if (sfx.customPool == null) 
                return handler.GetSource();
            
            GSSource source = sfx.customPool.GetObject().GetComponent<GSSource>();
            handler.RegisterSource(source);
            return source;
        }

        ////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        ////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            var weights = new List<int>();
            for (int i = 0, count = sfxList.Count; i < count; ++i)
            {
                weights.Add(sfxList[i].weight);
            }
            accumulatedWeights = RandomUtils.GetAccumulatedWeightList(weights, out totalWeight);
        }
#endif
    }
}