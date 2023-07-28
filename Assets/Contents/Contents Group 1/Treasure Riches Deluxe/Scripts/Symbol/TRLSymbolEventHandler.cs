using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.TRL.Symbol
{
    public class TRLSymbolEventHandler : DefaultSymbolEventHandler
    {
        public SpriteRenderer jackpotSymbolRenderer;

        public List<Sprite> jackpotSymbolSprite = new List<Sprite>();

        public override void Apply(BaseSymbol symbol)
        {
            base.Apply(symbol);
            if (symbol.symbolIndex == 12)
            {
                if (symbol.symbolInfo.customData != null && symbol.symbolInfo.customData.ContainsKey("jackpot"))
                {
                    int jackpotIndex = (int)symbol.symbolInfo.customData["jackpot"];
                    jackpotSymbolRenderer.sprite = jackpotSymbolSprite[jackpotIndex];
                }
                else
                {
                    jackpotSymbolRenderer.sprite = jackpotSymbolSprite[Random.Range(0, 3)];
                }
            }
        }

        public override void Change(BaseSymbol symbol)
        {
            base.Change(symbol);
        }

        public override void Clear(BaseSymbol symbol)
        {
            base.Clear(symbol);
        }

        public override void Restore(BaseSymbol target, BaseSymbol source)
        {
            base.Restore(target, source);
        }
    }
}
