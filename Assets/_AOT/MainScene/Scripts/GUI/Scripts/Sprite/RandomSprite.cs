using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class RandomSprite : MonoBehaviour
    {
        public SpriteRenderer target;
        [Serializable]
        public class SpriteWeight
        {
            public Sprite sprite;
            public int weight;
        }
        public List<SpriteWeight> spriteWeights;
        public int randomCount;
        public List<Sprite> randomSprites;

        [SerializeField]
        [Range(0f, 1f)]
        private float normalizedPosition;
        public float NormalizedPosition
        {
            get { return normalizedPosition; }
            set
            {
                normalizedPosition = Mathf.Clamp01(value);
                int index = Mathf.FloorToInt(normalizedPosition * (randomCount - 1));
                if (target != null) target.sprite = randomSprites[index];
            }
        }
        
        [ContextMenu("Create")]
        public void CreateRandomSprites()
        {
            int count = spriteWeights.Count;
            List<int> weights = new List<int>(count);
            for (int i = 0; i < count; ++i)
            {
                weights.Add(spriteWeights[i].weight);
            }
            int totalWeight = 0;
            weights = RandomUtils.GetAccumulatedWeightList(weights, out totalWeight);

            randomSprites = new List<Sprite>(randomCount);
            int lastIndex = -1;
            for (int i = 0; i < randomCount; ++i)
            {
                int newIndex;
                do
                {
                    newIndex = RandomUtils.WeightRandom(weights, totalWeight);    
                } while (newIndex == lastIndex);

                randomSprites.Add(spriteWeights[newIndex].sprite);
                lastIndex = newIndex;
            }
        }

        private void OnEnable()
        {
            CreateRandomSprites();
        }

        private void OnDidApplyAnimationProperties()
        {
            Validate();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
        
        private void OnValidate()
        {
            if (randomSprites != null && randomSprites.Count > 0)
                Validate();
        }

        private void Validate()
        {
            NormalizedPosition = normalizedPosition;
        }
    }
}