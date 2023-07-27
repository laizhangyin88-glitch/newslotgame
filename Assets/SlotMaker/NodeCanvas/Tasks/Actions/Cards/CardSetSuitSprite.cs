using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class CardSetSuitSprite : ActionTask
    {
        public BBParameter<GameObject> image;
        public BBParameter<int> suit;

        protected override string info { get { return string.Format("{0}.sprite = {1}", image, suit); } }

        protected override void OnExecute()
        {
            var _image = image.value.GetComponent<Image>();
            _image.sprite = CustomCardData.Instance.cardAssets.suitSprites[suit.value];
            _image.color = CustomCardData.Instance.cardAssets.suitColors[suit.value];
            
            EndAction();
        }
    }
}
