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
    public class BossRaidersSmallMonsterController : BossRaidersMonsterBase
    {
        private ContextElement partsEyeElement;
        private ContextElement partsEyeLineElement;
        private ContextElement partsCrownElement;
        private ContextElement partsFeatherFElement;
        private ContextElement partsFeatherLElement;
        private ContextElement partsFeatherRElement;

        private CanvasRendererProperty partsEyeCanvasRenderer;
        private CanvasRendererProperty partsEyeLineCanvasRenderer;
        private CanvasRendererProperty partsCrownFeatherCanvasRenderer;
        private CanvasRendererProperty partsCrownCanvasRenderer;
        private CanvasRendererProperty partsFeatherFCanvasRenderer;
        private CanvasRendererProperty partsFeatherLCanvasRenderer;
        private CanvasRendererProperty partsFeatherRCanvasRenderer;

        protected override void InitProperty()
        {
            base.InitProperty();

            partsEyeElement = ContextUtils.FindElement(scaleElement, "Eye Patch", ContextSearchingType.ChildrenSearch);
            partsEyeLineElement = ContextUtils.FindElement(partsEyeElement, "Eye Patch Line", ContextSearchingType.ChildrenSearch);
            partsCrownElement = ContextUtils.FindElement(scaleElement, "Crown", ContextSearchingType.ChildrenSearch);

            partsEyeCanvasRenderer = partsEyeElement.GetComponent<CanvasRendererProperty>();
            partsEyeLineCanvasRenderer = partsEyeLineElement.GetComponent<CanvasRendererProperty>();
            partsCrownFeatherCanvasRenderer = ContextUtils.FindElement(partsCrownElement, "Feather", ContextSearchingType.ChildrenSearch).GetComponent<CanvasRendererProperty>();

            partsFeatherFElement = ContextUtils.FindElement(scaleElement, "Feather Deco F", ContextSearchingType.ChildrenSearch);
            partsFeatherLElement = ContextUtils.FindElement(scaleElement, "Feather Deco L", ContextSearchingType.ChildrenSearch);
            partsFeatherRElement = ContextUtils.FindElement(scaleElement, "Feather Deco R", ContextSearchingType.ChildrenSearch);

            partsFeatherFCanvasRenderer = partsFeatherFElement.GetComponent<CanvasRendererProperty>();
            partsFeatherLCanvasRenderer = partsFeatherLElement.GetComponent<CanvasRendererProperty>();
            partsFeatherRCanvasRenderer = partsFeatherRElement.GetComponent<CanvasRendererProperty>();
        }

        protected override void InitBossScale()
        {
            float scaleValue = isMeta ? (float)BossRaidersUtils.CurrentBossScale : monsterData.scale;
            scaleElement.transform.localScale = new Vector3(scaleValue, scaleValue, 1.0f);
        }

        protected override void InitBossColor()
        {
            switch (colorType)
            {
                case BossRaidersBossColorType.BRONZE:
                    SetParsColor(new Color32(214, 115, 59, 255));   // #D6733B
                    break;
                case BossRaidersBossColorType.SILVER:
                    SetParsColor(new Color32(188, 188, 188, 255));  // #BCBCBC
                    break;
                case BossRaidersBossColorType.GOLD:
                    SetParsColor(new Color32(255, 208, 27, 255));   // #FFD01B
                    break;
                case BossRaidersBossColorType.EMERALD:
                    SetParsColor(new Color32(36, 255, 95, 255));    // #24FF5F
                    break;
                case BossRaidersBossColorType.RUBY:
                    SetParsColor(new Color32(255, 37, 37, 255));    // #FF2525
                    break;
                case BossRaidersBossColorType.SAPPHIRE:
                    SetParsColor(new Color32(36, 131, 255, 255));   // #2483FF
                    break;
                case BossRaidersBossColorType.DIAMOND:
                    SetParsColor(new Color32(72, 246, 254, 255));   // #48F6FE
                    break;
            }
        }

        protected override void InitBossParts()
        {
            switch (type)
            {
                case BossRaidersBossType.SMALL_1:
                    partsFeatherFElement.gameObject.SetActive(true);
                    partsFeatherLElement.gameObject.SetActive(false);
                    partsFeatherRElement.gameObject.SetActive(false);

                    partsEyeElement.gameObject.SetActive(false);

                    partsCrownElement.gameObject.SetActive(false);
                    break;
                case BossRaidersBossType.SMALL_2:
                    partsFeatherFElement.gameObject.SetActive(true);
                    partsFeatherLElement.gameObject.SetActive(true);
                    partsFeatherRElement.gameObject.SetActive(true);

                    partsEyeElement.gameObject.SetActive(true);

                    partsCrownElement.gameObject.SetActive(false);
                    break;
                case BossRaidersBossType.SMALL_3:
                    partsFeatherFElement.gameObject.SetActive(false);
                    partsFeatherLElement.gameObject.SetActive(false);
                    partsFeatherRElement.gameObject.SetActive(false);

                    partsEyeElement.gameObject.SetActive(false);

                    partsCrownElement.gameObject.SetActive(true);
                    break;
            }
        }

        private void SetParsColor(Color32 color32)
        {
            partsEyeCanvasRenderer.color = color32;
            partsEyeLineCanvasRenderer.color = color32;
            partsCrownFeatherCanvasRenderer.color = color32;
            partsFeatherFCanvasRenderer.color = color32;
            partsFeatherLCanvasRenderer.color = color32;
            partsFeatherRCanvasRenderer.color = color32;
        }

        public override void HitMonster(bool isAlive)
        {
            BossRaidersHitType hitType = isMeta ? BossRaidersUtils.GetHitType() : BossRaidersUtils.GetDealHitType();
            switch (hitType)
            {
                case BossRaidersHitType.BIG:
                case BossRaidersHitType.MEGA:
                case BossRaidersHitType.EPIC:
                    GSManager.Instance.GetHandler(isAlive ? BossRaidersUtils.Sounds.BOSS_RAIDERS_HIT_SMALL_BOSS : BossRaidersUtils.Sounds.BOSS_RAIDERS_DISAPPEAR_SMALL_BOSS).Play();
                    break;
                case BossRaidersHitType.DEFAULT:
                    if (!isAlive)
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DISAPPEAR_SMALL_BOSS).Play();
                    break;
            }
            
            base.HitMonster(isAlive);
        }

        public override void CreateMonsterSound()
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_NEW_SMALL_BOSS).Play();
        }
    }
}