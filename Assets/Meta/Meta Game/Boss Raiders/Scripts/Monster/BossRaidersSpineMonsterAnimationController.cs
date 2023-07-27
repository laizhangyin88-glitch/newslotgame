using UnityEngine;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSpineMonsterAnimationController : MonoBehaviour
    {
        private BossRaidersMonsterBase ownerController;

        public void OnInit(BossRaidersMonsterBase owner)
        {
            ownerController = owner;
        }

        private void EventCallAnimationAppear()
        {
            if (ownerController != null)
                ownerController.EventCallAnimationAppear();
        }

        private void EventCallAnimationHit()
        {
            if (ownerController != null)
                ownerController.EventCallAnimationHit();
        }

        private void EventCallAnimationActive()
        {
            if (ownerController != null)
                ownerController.EventCallAnimationActive();
        }

        private void EventCallAnimationDeactive()
        {
            if (ownerController != null)
                ownerController.EventCallAnimationDeactive();
        }
    }
}