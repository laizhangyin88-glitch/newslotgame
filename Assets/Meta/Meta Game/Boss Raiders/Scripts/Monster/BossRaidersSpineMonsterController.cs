using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using Spine.Unity;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSpineMonsterController : BossRaidersMonsterBase
    {
        public GameObject spineObject;

        private SkeletonMecanim skeletonMecanim;
        private BossRaidersSpineMonsterAnimationController spineAnimController;

        protected override void InitAnimator()
        {
            if (spineObject != null)
            {
                rootAnimator = spineObject.GetComponent<Animator>();
                skeletonMecanim = spineObject.GetComponent<SkeletonMecanim>();
                spineAnimController = spineObject.GetComponent<BossRaidersSpineMonsterAnimationController>();
                spineAnimController?.OnInit(this);
            }
        }

        protected override void InitProperty()
        {
            base.InitProperty();
        }

        protected override void InitBossScale()
        {
            float scaleValue = monsterData?.scale ?? 1.0f;
            scaleElement.transform.localScale = new Vector3(scaleValue, scaleValue, 1.0f);
        }

        protected override void InitBossColor()
        {
            if (skeletonMecanim != null)
            {
                string skinName = monsterData.skinName;
                if (!string.IsNullOrEmpty(skinName))
                {
                    skeletonMecanim.initialSkinName = skinName;
                    skeletonMecanim.skeleton.SetSkin(skinName);
                    skeletonMecanim.skeleton.SetSlotsToSetupPose();
                }
            }
        }

        public override void HitMonster(bool isAlive)
        {
            string soundKey = "";
            BossRaidersHitType hitType = isMeta ? BossRaidersUtils.GetHitType() : BossRaidersUtils.GetDealHitType();
            switch (hitType)
            {
                case BossRaidersHitType.BIG:
                case BossRaidersHitType.MEGA:
                case BossRaidersHitType.EPIC:
                    if (monsterData.isBigSize)
                        soundKey = isAlive ? BossRaidersUtils.Sounds.BOSS_RAIDERS_HIT_BIG_BOSS : GetDisappearSoundKey();
                    else
                        soundKey = isAlive ? BossRaidersUtils.Sounds.BOSS_RAIDERS_HIT_SMALL_BOSS : GetDisappearSoundKey();
                    GSManager.Instance.GetHandler(soundKey).Play();
                    break;
                case BossRaidersHitType.DEFAULT:
                    if (!isAlive)
                        GSManager.Instance.GetHandler(GetDisappearSoundKey()).Play();
                    break;
            }
            base.HitMonster(isAlive);
        }

        public override void CreateMonsterSound()
        {
            GSManager.Instance.GetHandler(string.Format("Meta_BossRaiders_NewBoss_Lv{0}", monsterData.bossIndex)).Play();
        }

        private string GetDisappearSoundKey()
        {
            return string.Format("Meta_BossRaiders_Disappear_Boss_Lv{0}", monsterData.bossIndex);
        }
    }
}