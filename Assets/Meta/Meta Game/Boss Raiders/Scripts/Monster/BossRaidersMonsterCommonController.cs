using SlotMaker;
using UnityEngine;
using UnityEngine.Events;

namespace BagelCode.BossRaiders
{
    public class BossRaidersMonsterCommonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement monsterAreaElement;

        // progress
        private ContextElement progressAnchorElement;
        private ContextElement energySliderElement;
        private ContextElement energyGreen;
        private ContextElement energyRed;
        private ContextElement textElement;
        private ContextElement levelTextElement;
        // Deal
        private ContextElement dealProgressFillElement;

        private BossRaidersMonsterBase monsterController = null;

        private UnityAction<string> ownerAnimatorCallback;

        private MonsterData monsterData = null;

        private bool isMeta = true;

        public bool OnInit(string bundle, string asset, MonsterData _monsterData, bool _isMeta = true)
        {
            isMeta = _isMeta;
            monsterData = _monsterData;
            InitProperty();
            bool isSuccess = InitMonster(bundle, asset);

            if (isMeta)
                InitMetaInfo();
            else
                InitDealInfo();

            return isSuccess;
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            monsterAreaElement = ContextUtils.FindElement(rootElement, "Monster Area", ContextSearchingType.ChildrenSearch);

            progressAnchorElement = ContextUtils.FindElement(rootElement, "Progress Anchor", ContextSearchingType.ChildrenSearch);
            energySliderElement = ContextUtils.FindElement(progressAnchorElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            levelTextElement = ContextUtils.FindElement(progressAnchorElement, "Text Level", ContextSearchingType.ChildrenSearch);
        }

        private bool InitMonster(string bundle, string asset)
        {
            bool isCreate = false;
            GameObject obj = MetaObjectUtils.MakePrefab(bundle, asset, monsterAreaElement.transform, null, "Monster Boss");
            if (obj != null)
            {
                monsterController = obj.GetComponent<BossRaidersMonsterBase>();
                if (monsterController == null)
                {
                    Destroy(obj);
                    return false;
                }
                monsterController.InitData(progressAnchorElement, monsterData, isMeta);
                monsterController.SetEventCall(EventCallAnimationAppear, EventCallAnimationHit, SetAnimationMonsterActive);
                
                return true;
            }
            return isCreate;
        }

        private void InitMetaInfo()
        {
            energyGreen = ContextUtils.FindElement(energySliderElement, "Fill Green", ContextSearchingType.ChildrenSearch);
            energyRed = ContextUtils.FindElement(energySliderElement, "Fill Red", ContextSearchingType.ChildrenSearch);
            textElement = ContextUtils.FindElement(progressAnchorElement, "Text Hp", ContextSearchingType.ChildrenSearch);

            // 2nd polishing
            textElement?.gameObject.SetActive(false);
            energyGreen?.gameObject.SetActive(false);
            energyRed?.gameObject.SetActive(true);
        }

        private void InitDealInfo()
        {
            dealProgressFillElement = ContextUtils.FindElement(energySliderElement, "Fill", ContextSearchingType.ChildrenSearch);
        }

        public void SetBossLevel(int level)
        {
            if (levelTextElement != null)
                MetaContextElementUtils.SetTextGlobal(levelTextElement, "BOSS_RAIDERS_MONSTER_LEVEL_TEXT", level);
        }

        public float SetBossHP(long hp)
        {
            float sliderValue = 1.0f;
            if (isMeta)
            {
                int currentTotalBossHP = BossRaidersUtils.CurrentTotalBossHP;
                sliderValue = (float)hp / (float)currentTotalBossHP;
                MetaContextElementUtils.SetFloatProperty(energySliderElement, sliderValue);
                MetaContextElementUtils.SetTextGlobal(textElement, "BOSS_RAIDERS_BOSS_HP_TEXT", hp, currentTotalBossHP);
            }
            else
            {
                long currentTotalBossHP = BossRaidersUtils.GetCurrentTotalBossHP();
                sliderValue = (float)hp / (float)currentTotalBossHP;
                MetaContextElementUtils.SetFloatProperty(energySliderElement, sliderValue);
            }
            // color check
            //bool isGreen = sliderValue > 0.1f;
            //energyGreen.gameObject.SetActive(isGreen);
            //energyRed.gameObject.SetActive(!isGreen);

            return sliderValue;
        }

        public void HitMonster(bool isAlive)
        {
            monsterController?.HitMonster(isAlive);
        }

        public void SetAnimator(string paramName)
        {
            monsterController?.SetAnimator(paramName);
        }

        public void SetAnimatorCallback(UnityAction<string> animatorCallback)
        {
            ownerAnimatorCallback = animatorCallback;
        }

        public void SetAnimationMonsterActive(bool isActive)
        {
            rootAnimator?.SetBool("isActive", isActive);
        }

        public void CreateMonsterSound()
        {
            monsterController?.CreateMonsterSound();
        }

        public virtual void EventCallAnimationAppear()
        {
            if (ownerAnimatorCallback != null)
                ownerAnimatorCallback.Invoke("Appear");
        }

        public virtual void EventCallAnimationHit()
        {
            if (ownerAnimatorCallback != null)
                ownerAnimatorCallback.Invoke("Attack");
        }
    }
}