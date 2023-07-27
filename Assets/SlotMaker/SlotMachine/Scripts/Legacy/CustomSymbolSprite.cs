using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class CustomSymbolSprite : MonoBehaviour 
    {
        public int targetIndex;
        public List<Sprite> sprites;

        private int _customSymbol;
        public int customSymbol
        {
            get { return _customSymbol; }
            set 
            {
                ContentCustomData.Instance.symbolSprite.sprites[targetIndex] = sprites[value];
                _customSymbol = value;
            }
        }
    }
}
