using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.LuckyFive
{
    [CreateAssetMenu(fileName="New LuckyFiveCardAssets", menuName="Meta/ScriptableObject/LuckyFiveCardAssets")]
    public class LuckyFiveCardAssets : ScriptableObject
    {
        public List<LuckyFiveCardInfo> cards;
        public List<Sprite> numberSprites;
        public List<Sprite> suitSprites;
        public List<Color> suitColors;
        // public List<Sprite> imageSprites;

        [ContextMenu("Setup")]
        void Setup()
        {
            cards = new List<LuckyFiveCardInfo>();
            int index = 0;
            for (int number = 1; number < 14; ++number)
            {
                for (int suit = 0; suit < 4; ++suit)
                {
                    cards.Add(new LuckyFiveCardInfo
                    {
                        index = index++,
                        number = number,
                        suit = suit
                    });
                }
            }
        }
    }
}
