using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSMultiplierSubSymbolController : MonoBehaviour
    {
        public WeightRandomGeneratorBase randomGenerator;
        public List<Sprite> spriteList;
        public List<Sprite> winSpriteList;
        public List<Sprite> blurSpriteList;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer winSpriteRenderer;
        public SpriteRenderer blurSpriteRenderer;
        public GameObject multiplierSubSymbol;
        public int multiplierIndex;

        public void Apply(BaseSymbol symbol)
        {
            int slotIndex = symbol.reel.slotMachine.slotIndex;
            SymbolInfo symbolInfo = symbol.symbolInfo;

            if (symbolInfo.customData == null || !symbolInfo.customData.Keys.Contains(slotIndex.ToString()))
            {
                multiplierIndex = randomGenerator.TakeOne();
            }
            else
            {
                int symbolMultiplier = (int) symbolInfo.customData[slotIndex.ToString()];

                switch (symbolMultiplier)
                {
                    case 1 :
                        multiplierIndex = 0;
                        break;
                    case 2 :
                        multiplierIndex = 1;
                        break;
                    case 3 :
                        multiplierIndex = 2;
                        break;
                    case 5 :
                        multiplierIndex = 3;
                        break;
                    default :
                        break;
                }
            }

            if (multiplierIndex > 0)
            {
                multiplierSubSymbol.SetActive(true);
                spriteRenderer.sprite = spriteList[multiplierIndex - 1];
                winSpriteRenderer.sprite = winSpriteList[multiplierIndex - 1];
                blurSpriteRenderer.sprite = blurSpriteList[multiplierIndex - 1];
            }
            else
            {
                multiplierSubSymbol.SetActive(false);
            }
        }

        public void Win()
        {
            if (multiplierIndex > 0)
            {
                spriteRenderer.sortingLayerID = SortingLayer.NameToID("Midground");
                multiplierSubSymbol.GetComponent<Animator>().Play(Animator.StringToHash("Win"), -1, 0f);
            }
        }
        public void Skip()
        {
            if (multiplierIndex > 0)
            {
                spriteRenderer.sortingLayerID = SortingLayer.NameToID("Base");
                multiplierSubSymbol.GetComponent<Animator>().Play(Animator.StringToHash("Idle"), -1, 0f);
            }
        }
    }
}
