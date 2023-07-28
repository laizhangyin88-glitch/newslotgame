using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSWildMultiplierContorller : MonoBehaviour
    {
        [Serializable]
        public struct SpriteList
        {
            [SerializeField]
            public List<Sprite> spriteList;
        }
        public WeightRandomGeneratorBase randomGenerator;
        [SerializeField]
        public List<SpriteList> spriteTable;
        public List<SpriteList> winSpriteTable;
        public List<SpriteList> blurSpriteTable;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer winSpriteRenderer;
        public SpriteRenderer blurSpriteRenderer;
        public GameObject multiplierSubSymbol;
        public int multiplierIndex;

        public void Apply(BaseSymbol symbol)
        {
            int superBonusIndex = BlackboardUtils.FindValue<int>(null, "./game/superBonusIndex");
            
            List<Sprite> spriteList = spriteTable[superBonusIndex].spriteList;
            List<Sprite> winSpriteList = winSpriteTable[superBonusIndex].spriteList;
            List<Sprite> blurSpriteList = blurSpriteTable[superBonusIndex].spriteList;

            int slotIndex = symbol.reel.slotMachine.slotIndex;
            SymbolInfo symbolInfo = symbol.symbolInfo;
            int nextSymbolIndex = -1;
            BaseSymbol nextSymbol = null;

            if (symbol.row + 1 < 7)
            {
                nextSymbol = symbol.reel.slotMachine.GetSymbol(symbol.column, symbol.row + 1);
                nextSymbolIndex = nextSymbol.symbolIndex;
            }

            bool isSuperBonusInit = BlackboardUtils.FindValue<bool>(null, "./customData/isSuperBonusInit");
            if (isSuperBonusInit && symbol.row > 0)
            {
                nextSymbol = symbol.reel.slotMachine.GetSymbol(symbol.column, symbol.row - 1);
                nextSymbolIndex = nextSymbol.symbolIndex;
            }

            if (nextSymbolIndex == symbol.symbolIndex)
            {
                multiplierIndex = nextSymbol.GetComponentInChildren<NDSWildMultiplierContorller>().multiplierIndex;
            }
            else if (superBonusIndex == 0)
            {
                multiplierIndex = 0;
            }
            else if (symbolInfo.customData == null || !symbolInfo.customData.Keys.Contains(slotIndex.ToString()))
            {
                multiplierIndex = randomGenerator.TakeOne();
            }
            else
            {
                int symbolMultiplier = (int) symbolInfo.customData[slotIndex.ToString()];
                if (superBonusIndex == 1)
                {
                    switch (symbolMultiplier)
                    {
                        case 1 :
                            multiplierIndex = 0;
                            break;
                        case 2 :
                            multiplierIndex = 1;
                            break;
                        default :
                            break;
                    }
                }
                else if (superBonusIndex == 2)
                {
                    switch (symbolMultiplier)
                    {
                        case 1 :
                            multiplierIndex = 0;
                            break;
                        case 3 :
                            multiplierIndex = 1;
                            break;
                        default :
                            break;
                    }
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
