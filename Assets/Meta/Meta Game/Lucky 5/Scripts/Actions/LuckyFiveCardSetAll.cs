using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.LuckyFive.Tasks.Actions
{
    [Category("★ BagelCode/LuckyFive")]
    public class LuckyFiveCardSetAll : ActionTask
    {
        public BBParameter<GameObject> bigSuitImage;
        public BBParameter<GameObject> numberImage;
        public BBParameter<GameObject> numberSuitImage;
        public BBParameter<GameObject> fullSuitImage;
        public BBParameter<int> number;
        public BBParameter<int> suit;
        public BBParameter<bool> useFullSuit;

        protected override string info { get { return string.Format("{0}.sprite = {1}", numberImage, suit); } }

        protected override void OnExecute()
        {
            if(useFullSuit.value)
            {
                bigSuitImage.value.SetActive(false);
                numberImage.value.SetActive(false);
                numberSuitImage.value.SetActive(false);
                fullSuitImage.value.SetActive(true);

                var _image = fullSuitImage.value.GetComponent<Image>();
                _image.sprite = LuckyFiveCustomCardData.Instance.cardAssets.suitSprites[suit.value];
                _image.color = LuckyFiveCustomCardData.Instance.cardAssets.suitColors[suit.value];
            }
            else
            {
                bigSuitImage.value.SetActive(true);
                numberImage.value.SetActive(true);
                numberSuitImage.value.SetActive(true);
                fullSuitImage.value.SetActive(false);

                var _image = bigSuitImage.value.GetComponent<Image>();
                _image.sprite = LuckyFiveCustomCardData.Instance.cardAssets.suitSprites[suit.value];
                _image.color = LuckyFiveCustomCardData.Instance.cardAssets.suitColors[suit.value];

                _image = numberImage.value.GetComponent<Image>();
                _image.sprite = LuckyFiveCustomCardData.Instance.cardAssets.numberSprites[number.value];
                _image.color = LuckyFiveCustomCardData.Instance.cardAssets.suitColors[suit.value];

                _image = numberSuitImage.value.GetComponent<Image>();
                _image.sprite = LuckyFiveCustomCardData.Instance.cardAssets.suitSprites[suit.value];
                _image.color = LuckyFiveCustomCardData.Instance.cardAssets.suitColors[suit.value];
            }

            EndAction();
        }
    }
}
