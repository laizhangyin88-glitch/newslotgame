using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Cards
{
    [CreateAssetMenu(fileName="New CardAssets", menuName="SlotMaker/ScriptableObject/CardAssets")]
    public class CardAssets : ScriptableObject
    {
        public List<CardInfo> cards;
        public List<Sprite> numberSprites;
        public List<Sprite> suitSprites;
        public List<Color> suitColors;
        public List<Sprite> imageSprites;

        [ContextMenu("Setup")]
        void Setup()
        {
            cards = new List<CardInfo>();
            int index = 0;
            for (int number = 1; number < 14; ++number)
            {
                for (int suit = 0; suit < 4; ++suit)
                {
                    cards.Add(new CardInfo
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
