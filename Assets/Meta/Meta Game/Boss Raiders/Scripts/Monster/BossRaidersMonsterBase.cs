using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.BossRaiders
{
    public class BossRaidersMonsterBase : MonoBehaviour
    {
        protected ContextElement rootElement;
        protected Animator rootAnimator;

        protected BossRaidersBossType type;
        protected BossRaidersBossColorType colorType;
        protected float bossScale = 1.0f;

        protected ContextElement scaleElement;
        protected ContextElement progressAnchorElement;

        protected MonsterData monsterData = null;

        protected bool isInit = false;
        protected bool isMeta = true;

        // Animation Event Call
        public UnityAction appearEventCallback;
        public UnityAction hitEventCallback;
        public UnityAction<bool> activeEventCallback;

        protected virtual void InitBossScale() { }
        protected virtual void InitBossColor() { }
        protected virtual void InitBossParts() { }

        public virtual void CreateMonsterSound() { }

        protected virtual void InitProperty()
        {
            scaleElement = ContextUtils.FindElement(rootElement, "Scale Anchor", ContextSearchingType.ChildrenSearch);
        }

        private void InitPropertyBase()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            type = monsterData?.type ?? BossRaidersUtils.CurrentBossType;
            colorType = monsterData?.colorType ?? BossRaidersUtils.CurrentBossColorType;
            bossScale = monsterData?.scale ?? (float)BossRaidersUtils.CurrentBossScale;
        }

        protected virtual void InitAnimator()
        {
            rootAnimator = gameObject.GetComponent<Animator>();
        }

        private void InitBossSetting()
        {
            InitBossScale();
            InitBossColor();
            InitBossParts();
        }

        public virtual void InitData(ContextElement progressElement, MonsterData _monsterData, bool _isMeta)
        {
            progressAnchorElement = progressElement;
            monsterData = _monsterData;
            isMeta = _isMeta;
            if (isInit) return;

            InitPropertyBase();
            InitAnimator();
            InitProperty();
            InitBossSetting();

            SetAnimator("Appear");

            isInit = true;
        }

        public virtual void HitMonster(bool isAlive)
        {
            SetAnimator(isAlive ? "Hit" : "Disappear");

            BossRaidersHitType hitType = isMeta ? BossRaidersUtils.GetHitType() : BossRaidersUtils.GetDealHitType();
            switch (hitType)
            {
                case BossRaidersHitType.DEFAULT:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_INJURED_NORMAL).Play();
                    break;
                case BossRaidersHitType.BIG:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_INJURED_BIGHIT).Play();
                    break;
                case BossRaidersHitType.MEGA:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_INJURED_MEGAHIT).Play();
                    break;
                case BossRaidersHitType.EPIC:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BATTLE_INJURED_EPICHIT).Play();
                    break;
            }
        }

        public void SetEventCall(UnityAction appearEvent, UnityAction hitEvent, UnityAction<bool> activeEvent)
        {
            appearEventCallback = appearEvent;
            hitEventCallback = hitEvent;
            activeEventCallback = activeEvent;
        }

        public void SetAnimator(string paramName)
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger(paramName);
        }

        public void EventCallAnimationAppear()
        {
            appearEventCallback?.Invoke();
        }

        public void EventCallAnimationHit()
        {
            hitEventCallback?.Invoke();
        }

        public void EventCallAnimationActive()
        {
            EventCallAnimationActive(true);
        }

        public void EventCallAnimationDeactive()
        {
            EventCallAnimationActive(false);
        }

        protected void EventCallAnimationActive(bool isActive)
        {
            activeEventCallback?.Invoke(isActive);
        }
    }
}