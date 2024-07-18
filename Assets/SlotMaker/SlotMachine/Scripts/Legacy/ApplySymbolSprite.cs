using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class ApplySymbolSprite : MonoBehaviour
    {
        public SpriteRenderer image;
        public int additionalSortingOrder;

        public void Apply(BaseSymbol symbol)
        {
            ContentCustomData.Instance.symbolSprite.Apply(symbol, image, additionalSortingOrder);
        }

        public void ApplySprite(BaseSymbol symbol)
        {
            ContentCustomData.Instance.symbolSprite.ApplySprite(symbol, image);
        }

        public void ApplySortingOrder(BaseSymbol symbol)
        {
            ContentCustomData.Instance.symbolSprite.ApplySortingOrder(symbol, image, additionalSortingOrder);
        }
    }
}
