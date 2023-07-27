using UnityEngine;
using SlotMaker;

namespace BagelCode.BossRaiders
{
    public abstract class BossRaidersCharacterBase : MonoBehaviour
    {
        protected Animator rootAnimator;

        protected virtual void InitProperty() { }

        public virtual void InitData()
        {
            InitProperty();
        }

        public abstract void Attack(bool isAttack);
        public abstract void SetAnimator(string paramName, bool isActive);
        public abstract void SetAnimatorTrigger(string paramName);
    }
}