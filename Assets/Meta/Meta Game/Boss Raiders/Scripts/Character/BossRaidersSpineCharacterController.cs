using UnityEngine;
using SlotMaker;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSpineCharacterController : BossRaidersCharacterBase
    {
        public GameObject spineObject;

        private bool isInit = false;

        protected override void InitProperty()
        {
            if (isInit) return;
            base.InitProperty();

            if (spineObject != null)
                rootAnimator = spineObject.GetComponent<Animator>();

            isInit = false;
        }

        public override void InitData()
        {
            base.InitData();
            //SetAnimator("Active", true);
        }

        public override void Attack(bool isAttack)
        {
            if (isAttack)
            {
                SetAnimatorTrigger("Attack");
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_ATTACK).Play();
            }
        }

        public override void SetAnimator(string paramName, bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool(paramName, isActive);
        }

        public override void SetAnimatorTrigger(string paramName)
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger(paramName);
        }
    }
}