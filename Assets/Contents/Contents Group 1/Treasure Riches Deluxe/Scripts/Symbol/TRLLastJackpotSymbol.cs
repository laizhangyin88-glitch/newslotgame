using GameStudio.Slot.TRL.Utillity;
using SlotMaker;

namespace GameStudio.Slot.TRL.Symbol
{
    public class TRLLastJackpotSymbol : SymbolBehaviour
    {
        private const int Stop = 0;
        private const int Win = 3;

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
            GetCachedObject(Stop + (int)symbol.symbolInfo.customData["jackpot"]).SetActive(true);
            PlayAnimation("Inactive");
        }

        public override void OnWin()
        {
            DisableAllCachedObjects();
            GetCachedObject(Win + (int)symbol.symbolInfo.customData["jackpot"]).SetActive(true);

            TRLUtillity.SendEvent("OnSoundEvent", "JackpotSymbolChange");
            PlayAnimation("Inactive");
        }
    }
}