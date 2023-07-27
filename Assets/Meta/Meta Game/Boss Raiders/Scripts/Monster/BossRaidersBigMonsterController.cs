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
    public class BossRaidersBigMonsterController : BossRaidersMonsterBase
    {
        private CanvasRendererProperty ponchoCanvasRenderer;
        private CanvasRendererProperty ponchoGradientCanvasRenderer;
        private CanvasRendererProperty ponchoColorCanvasRenderer;
        private CanvasRendererProperty HandDecoRCanvasRenderer;
        private CanvasRendererProperty HandDecoColorRCanvasRenderer;
        private CanvasRendererProperty HandDecoLCanvasRenderer;
        private CanvasRendererProperty HandDecoColorLCanvasRenderer;

        protected override void InitProperty()
        {
            base.InitProperty();

            ContextElement ponchoElement = ContextUtils.FindElement(scaleElement, "Poncho", ContextSearchingType.ChildrenSearch);
            ContextElement ponchoGradientElement = ContextUtils.FindElement(ponchoElement, "Poncho Gradient", ContextSearchingType.ChildrenSearch);
            ContextElement ponchoColorElement = ContextUtils.FindElement(ponchoElement, "Poncho Color", ContextSearchingType.ChildrenSearch);

            ponchoCanvasRenderer = ponchoElement.GetComponent<CanvasRendererProperty>();
            ponchoGradientCanvasRenderer = ponchoGradientElement.GetComponent<CanvasRendererProperty>();
            ponchoColorCanvasRenderer = ponchoColorElement.GetComponent<CanvasRendererProperty>();

            ContextElement handDecoRElement = ContextUtils.FindElement(scaleElement, "Hand Deco R", ContextSearchingType.ChildrenSearch);
            ContextElement handDecoColorRElement = ContextUtils.FindElement(handDecoRElement, "Hand Deco Color R", ContextSearchingType.ChildrenSearch);
            ContextElement handDecoLElement = ContextUtils.FindElement(scaleElement, "Hand Deco L", ContextSearchingType.ChildrenSearch);
            ContextElement handDecoColorLElement = ContextUtils.FindElement(handDecoLElement, "Hand Deco Color L", ContextSearchingType.ChildrenSearch);

            HandDecoRCanvasRenderer = handDecoRElement.GetComponent<CanvasRendererProperty>();
            HandDecoColorRCanvasRenderer = handDecoColorRElement.GetComponent<CanvasRendererProperty>();
            HandDecoLCanvasRenderer = handDecoLElement.GetComponent<CanvasRendererProperty>();
            HandDecoColorLCanvasRenderer = handDecoColorLElement.GetComponent<CanvasRendererProperty>();
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
                    SetColorPoncho(new Color32(164, 81, 33, 255));          // #A45121
                    SetColorPonchoColor(new Color32(226, 135, 82, 255));    // #E28752
                    SetColorPonchoGradient(new Color32(115, 48, 22, 255));  // #733016
                    break;
                case BossRaidersBossColorType.SILVER:
                    SetColorPoncho(new Color32(255, 255, 255, 255));        // #FFFFFF
                    SetColorPonchoColor(new Color32(60, 60, 60, 255));      // #3C3C3C
                    SetColorPonchoGradient(new Color32(168, 168, 168, 255));// #A8A8A8
                    break;
                case BossRaidersBossColorType.GOLD:
                    SetColorPoncho(new Color32(255, 162, 0, 255));          // #FFA200
                    SetColorPonchoColor(new Color32(226, 212, 0, 255));     // #FFD400
                    SetColorPonchoGradient(new Color32(226, 63, 0, 255));   // #E23F00
                    break;
                case BossRaidersBossColorType.EMERALD:
                    SetColorPoncho(new Color32(0, 224, 85, 255));           // #00E055
                    SetColorPonchoColor(new Color32(0, 255, 2, 255));       // #00FF02
                    SetColorPonchoGradient(new Color32(0, 145, 226, 255));  // #0091E2
                    break;
                case BossRaidersBossColorType.RUBY:
                    SetColorPoncho(new Color32(255, 3, 0, 255));            // #FF0300
                    SetColorPonchoColor(new Color32(255, 0, 28, 255));      // #FF0028
                    SetColorPonchoGradient(new Color32(226, 0, 123, 255));  // #E2007B
                    break;
                case BossRaidersBossColorType.SAPPHIRE:
                    SetColorPoncho(new Color32(52, 102, 255, 255));         // #3466FF
                    SetColorPonchoColor(new Color32(52, 76, 255, 255));     // #344CFF
                    SetColorPonchoGradient(new Color32(0, 181, 255, 255));  // #00B5FF
                    break;
                case BossRaidersBossColorType.DIAMOND:
                    ponchoCanvasRenderer.color = new Color32(126, 233, 255, 255);   // #7EE9FF
                    HandDecoLCanvasRenderer.color = new Color32(126, 233, 255, 255);// #7EE9FF
                    HandDecoRCanvasRenderer.color = new Color32(255, 126, 253, 255);// #FF7EFD
                    SetColorPonchoColor(new Color32(153, 250, 255, 255));   // #99FAFF
                    SetColorPonchoGradient(new Color32(255, 66, 218, 255)); // #FF42DA
                    break;
            }
            //SetProgressPos();
        }

        private void SetColorPoncho(Color32 color32)
        {
            ponchoCanvasRenderer.color = color32;
            HandDecoRCanvasRenderer.color = color32;
            HandDecoLCanvasRenderer.color = color32;
        }

        private void SetColorPonchoColor(Color32 color32)
        {
            ponchoColorCanvasRenderer.color = color32;
            HandDecoColorRCanvasRenderer.color = color32;
            HandDecoColorLCanvasRenderer.color = color32;
        }

        private void SetColorPonchoGradient(Color32 color32)
        {
            ponchoGradientCanvasRenderer.color = color32;
        }

        private void SetProgressPos()
        {
            switch (colorType)
            {
                case BossRaidersBossColorType.BRONZE:
                case BossRaidersBossColorType.SILVER:
                case BossRaidersBossColorType.GOLD:
                    progressAnchorElement.transform.localPosition = new Vector3(-16, 228, 0);
                    break;
                case BossRaidersBossColorType.EMERALD:
                case BossRaidersBossColorType.RUBY:
                case BossRaidersBossColorType.SAPPHIRE:
                    progressAnchorElement.transform.localPosition = new Vector3(-16, 248, 0);
                    break;
                case BossRaidersBossColorType.DIAMOND:
                    progressAnchorElement.transform.localPosition = new Vector3(-16, 258, 0);
                    break;
            }
        }

        public override void HitMonster(bool isAlive)
        {
            BossRaidersHitType hitType = isMeta ? BossRaidersUtils.GetHitType() : BossRaidersUtils.GetDealHitType();
            switch (hitType)
            {
                case BossRaidersHitType.BIG:
                case BossRaidersHitType.MEGA:
                case BossRaidersHitType.EPIC:
                    GSManager.Instance.GetHandler(isAlive ? BossRaidersUtils.Sounds.BOSS_RAIDERS_HIT_BIG_BOSS : BossRaidersUtils.Sounds.BOSS_RAIDERS_DISAPPEAR_BIG_BOSS).Play();
                    break;
                case BossRaidersHitType.DEFAULT:
                    if (!isAlive)
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DISAPPEAR_BIG_BOSS).Play();
                    break;
            }
            base.HitMonster(isAlive);
        }

        public override void CreateMonsterSound()
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_NEW_BIG_BOSS).Play();
        }
    }
}