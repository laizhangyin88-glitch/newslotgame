using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class SymbolSprite : MonoBehaviour
    {
        public List<Sprite> sprites;

        public virtual void Apply(BaseSymbol symbol, SpriteRenderer image, int additionalSortingOrder)
        {
            image.sprite = symbol.symbolInfo.link.isPivot ? sprites[symbol.symbolIndex] : null;
            image.sortingLayerName = ContentCustomData.Instance.symbolSortingOrder.sortingLayerName;
            image.sortingOrder = ContentCustomData.Instance.symbolSortingOrder.orders[symbol.symbolIndex] + additionalSortingOrder;
        }

        public virtual void ApplySprite(BaseSymbol symbol, SpriteRenderer image)
        {
            image.sprite = symbol.symbolInfo.link.isPivot ? sprites[symbol.symbolIndex] : null;
        }

        public virtual void ApplySortingOrder(BaseSymbol symbol, SpriteRenderer image, int additionalSortingOrder)
        {
            image.sortingLayerName = ContentCustomData.Instance.symbolSortingOrder.sortingLayerName;
            image.sortingOrder = ContentCustomData.Instance.symbolSortingOrder.orders[symbol.symbolIndex] + additionalSortingOrder;
        }
    }
}
