using UnityEngine;

namespace BagelCode.BossRaiders.Deal
{
    public class BossRaidersDealAnimationEventSender : MonoBehaviour
    {
        public BossRaidersDealSceneController dealMainController;

        public void SendDealUpdateRewardAnimation()
        {
            dealMainController?.OnUpdateRewardAnimation();
        }
    }
}