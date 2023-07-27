using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    public class BossRaidersCharacterController : BossRaidersCharacterBase
    {
        private bool isInit = false;

        //public Vector3 startPos = Vector3.zero;
        //public Vector3 endPos = Vector3.zero;

        protected override void InitProperty()
        {
            base.InitProperty();

            if (isInit) return;

            rootAnimator = gameObject.GetComponent<Animator>();

            isInit = true;
        }

        public override void InitData()
        {
            base.InitData();
            SetAnimator("Active", true);
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