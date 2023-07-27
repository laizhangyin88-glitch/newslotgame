using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.HOC
{
    public class HOCWildSymbolBehaviour : SymbolBehaviour
    {
        public const int EXPANDING_PREFAB_START_INDEX = 1;
        public const int WIN_PREFAB_START_INDEX = 5;
        public const int DEFAULT_LAYER_ORDER = 3;

        public const int SB_SPRITE_COUNT_PER_SIZE = 6;
        public const int SB_SPRITE_COUNT_PER_COPY_COUNT = 3;
        public const int SB_SPRITE_INDEX_START_OFFSET = 4;

        public const int SB_EXPANDING_PREFAB_INDEX_START_OFFSET= 9;
        public const int SB_WIN_PREFAB_INDEX_START_OFFSET = 12;

        public static bool IsSuperBonus
        {
            get => BlackboardUtils.FindValue<bool>(null, "./customData/isSuperBonus");
        }

        public static bool IsFreeGame
        {
            get => BlackboardUtils.FindValue<bool>(null, "./customData/isFreeSpin");
        }

        private int SymbolMultiplier
        {
            get
            {
                var deck =  ContentCustomData.Instance.slotDataList[symbol.slotMachine.slotIndex].deck;
                if (deck.deck == null || symbol.row < 0 || symbol.row >= symbol.slotMachine.RowCount) return 0;
                return deck.GetOriginalSymbol(symbol.column, symbol.row).multiplier;
            }
        }

        public override void OnEntry()
        {
        }

        public override void OnSkip()
        {
            animator.Play("Idle");
            int expandCount = symbol.symbolInfo.link.rowCount;
            var spriteRenderer = animator.GetComponentInChildren<SpriteRenderer>(true);
            spriteRenderer.sortingOrder = DEFAULT_LAYER_ORDER + expandCount - 1;

            if (IsSuperBonus &&  SymbolMultiplier > 1)
            {
                int copiedCount = GetCopyCountByMultiplier(SymbolMultiplier);
                var expandedSprite = GetSprite(copiedCount,expandCount,SymbolMultiplier);
                spriteRenderer.sprite = expandedSprite;

                GetCachedObject(0).SetActive(false);
                // expand prefab
                if(expandCount > 1) GetCachedObject(SB_EXPANDING_PREFAB_INDEX_START_OFFSET+expandCount -2).SetActive(false);
                // win prefab
                GetCachedObject(SB_WIN_PREFAB_INDEX_START_OFFSET+expandCount -1).SetActive(false);
            }
            else
            {
                var expandedSprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, expandCount - 1);
                spriteRenderer.sprite = expandedSprite;

                GetCachedObject(0).SetActive(false);
                GetCachedObject(EXPANDING_PREFAB_START_INDEX + expandCount - 1).SetActive(false);
                GetCachedObject(WIN_PREFAB_START_INDEX + expandCount - 1).SetActive(false);
            }
        }

        public override void OnStopEffect()
        {
            animator.Play("Invisible");
            GetCachedObject(0).SetActive(true);
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnWin()
        {
            animator.Play("Invisible");
            int expandCount = symbol.symbolInfo.link.rowCount;
            GetCachedObject(0).SetActive(false);
            GetCachedObject(1).SetActive(false);

            int symbolMultiplier = SymbolMultiplier;
            if (symbolMultiplier > 1)
            {
                if(expandCount > 1) GetCachedObject(SB_EXPANDING_PREFAB_INDEX_START_OFFSET+expandCount-2).SetActive(false);
                var winPrefab = GetCachedObject(SB_WIN_PREFAB_INDEX_START_OFFSET+expandCount-1);
                winPrefab.SetActive(false);
                winPrefab.SetActive(true);
                var animator = winPrefab.GetComponentInChildren<Animator>();
                animator.SetInteger("Multiplier", symbolMultiplier);
                animator.SetInteger("Copied Count",GetCopyCountByMultiplier(symbolMultiplier));
            }
            else
            {
                GetCachedObject(EXPANDING_PREFAB_START_INDEX + expandCount - 1).SetActive(false);
                var winPrefab = GetCachedObject(WIN_PREFAB_START_INDEX + expandCount - 1);
                winPrefab.SetActive(false);
                winPrefab.SetActive(true);
            }
        }

        public void Expand()
        {
            int expandCount = symbol.symbolInfo.link.rowCount;
            if (expandCount <= 1) return;

            animator.Play("Invisible");
            var spriteRenderer = animator.GetComponentInChildren<SpriteRenderer>(true);
            GetCachedObject(0).SetActive(false);
            Sprite expandedSprite = null;

            int symbolMultiplier = SymbolMultiplier;
            if (symbolMultiplier > 1)
            {
                int copiedCount = GetCopyCountByMultiplier(symbolMultiplier);
                expandedSprite = GetSprite(copiedCount, expandCount, symbolMultiplier);

                var expandPrefab = GetCachedObject(SB_EXPANDING_PREFAB_INDEX_START_OFFSET+expandCount-2);
                expandPrefab.SetActive(true);
                var animator = expandPrefab.GetComponentInChildren<Animator>();
                animator.SetInteger("Multiplier", symbolMultiplier);
                animator.SetInteger("Copied Count", GetCopyCountByMultiplier(symbolMultiplier));
            }
            else
            {
                var expandPrefab = GetCachedObject(EXPANDING_PREFAB_START_INDEX + expandCount - 1);
                expandPrefab.SetActive(true);
                expandedSprite = symbol.symbolAssets.GetSprite(symbol.symbolIndex, expandCount - 1);
            }

            spriteRenderer.sprite = expandedSprite;
            if (IsFreeGame) GSManager.Instance.GetHandler("Wild Symbol Expand FG").Play();
            else GSManager.Instance.GetHandler("Wild Symbol Expand").Play();
        }

        private Sprite GetSprite(int copiedCount, int expandCount, int multiplier)
        {
            if (multiplier <= 1)
            {
                return symbol.symbolAssets.GetSprite(symbol.symbolIndex, expandCount - 1);
            }
            else
            {
                var offsetByCopiedCount = copiedCount;
                if (copiedCount > 0) offsetByCopiedCount -= 1;
                var spriteIndex =   SB_SPRITE_INDEX_START_OFFSET + SB_SPRITE_COUNT_PER_SIZE *( expandCount -1 ) +
                                    SB_SPRITE_COUNT_PER_COPY_COUNT * offsetByCopiedCount + GetSpriteIndexOffsetByMultiplier(multiplier);
                return symbol.symbolAssets.GetSprite(symbol.symbolIndex, spriteIndex);
            }
        }

        private int GetSpriteIndexOffsetByMultiplier(int multiplier)
        {
            int copiedCount = GetCopyCountByMultiplier(multiplier);
            if (copiedCount == 1)
                switch (multiplier)
                {
                    case 2: return 0;
                    case 3: return 1;
                    default: return 2;
                }
            else if (copiedCount == 2)
                switch (multiplier)
                {
                    case 3: return 0;
                    case 5: return 1;
                    default: return 2;
                }
            else return 0;
        }

        private int GetCopyCountByMultiplier(int multiplier)
        {
            List<int> multiplierPerFlyingCount =
                BlackboardUtils.FindValue<List<int>>(null, "./bonus/response/multiplierPerCopyCount");

            for(int i=0;i<multiplierPerFlyingCount.Count;i++)
                if (multiplier == multiplierPerFlyingCount[i])
                    return i;

            return 0;
        }
}
}
