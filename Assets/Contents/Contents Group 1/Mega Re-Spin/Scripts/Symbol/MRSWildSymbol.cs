using GameStudio.Slot.MRS.Utility;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.MRS.Symbol
{
    public class MRSWildSymbol : SymbolBehaviour
    {
        private const int Stop = 0;
        private const int Win = 1;

        public override void OnEntry()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            DisableAllCachedObjects();
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            DisableAllCachedObjects();
            PlayAnimation("Inactive");
            if (symbol.column == 0 || symbol.column == 2)
            {
                MRSUtility.SendEvent("OnContentUIDetailEvent", "WILD_2X_STOP", symbol.column);
                MRSUtility.PlaySound("Wild Pay Symbol Land");
            }
            else
            {
                MRSUtility.SendEvent("OnContentUIDetailEvent", "WILD_STACK_STOP", symbol.column);
                MRSUtility.PlaySound("Wild Pay Land on 2th Reel");
            }
            if (symbol.symbolIndex == 0)
            {
                if (symbol.column == 0 || symbol.column == 2) GetCachedObject(2 + Stop).SetActive(true);
                else
                {
                    GetCachedObject(Stop).SetActive(true);
                }
            }
            else GetCachedObject(Stop).SetActive(true);
        }

        public override void OnWin()
        {
            DisableAllCachedObjects();
            if (symbol.symbolIndex == 0)
            {
                if (symbol.column == 0 || symbol.column == 2) GetCachedObject(2 + Win).SetActive(true);
                else
                {
                    GetCachedObject(Win).SetActive(true);
                }
            }
            else
            {
                GetCachedObject(Win).SetActive(true);
            }
            PlayAnimation("Inactive");
        }
    }
}
