using SlotMaker;

namespace BagelCode.BossRaiders.Deal
{
    public class BossRaidersDealCharacterBarController : BossRaidersCharacterBarBase
    {
        protected override void InitProperty() { }

        public override void SetEnergy(long power)
        {
            MetaContextElementUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_BOSS_RAIDERS_DEAL_POWER", power));
        }

        public override void SetEnergyAnimator(bool isActive) { }
    }
}