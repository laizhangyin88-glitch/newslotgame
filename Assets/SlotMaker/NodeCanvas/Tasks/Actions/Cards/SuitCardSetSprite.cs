using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class SuitCardSetSprite : ActionTask
    {
        public BBParameter<int> suit;
        public BBParameter<GameObject> card;

        protected override string info { get { return string.Format("SuitCardSetSprite"); } }

        protected override void OnExecute()
        {
            var _imageObj = card.value.GetComponent<Blackboard>().GetValue<GameObject>("cardImage");
            var _colors = card.value.GetComponent<Blackboard>().GetValue<List<Color>>("colors");
            var _sprites = card.value.GetComponent<Blackboard>().GetValue<List<Sprite>>("sprites");
            var _image = _imageObj.GetComponent<Image>();

            _image.sprite = _sprites[suit.value];
            _image.color = _colors[suit.value];

            EndAction();
        }
    }
}
