using SlotMaker;

namespace BagelCode.BossRaiders
{
    public class BossRaidersStageClear
    {
        private ContextElement rewardElement;

        public void OnInit(ContextElement _rewardElement, string initText)
        {
            rewardElement = _rewardElement;
            SetText(initText);
        }

        public void UpdateBossRewardPoint(string text)
        {
            SetText(text);
        }

        private void SetText(string text)
        {
            MetaContextElementUtils.SetText(rewardElement, text);
        }
    }
}