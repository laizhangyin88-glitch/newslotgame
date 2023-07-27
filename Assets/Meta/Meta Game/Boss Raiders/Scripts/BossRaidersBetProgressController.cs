using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersBetProgressController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextSlider contextSlider;
        private ContextElement textElement;
        private ContextElement[] iconElements;

        private float displayTime = 1.5f;
        private float checkTime = 0.0f;
        private float waitTime = 0.1f;
        private float gaugeTime = 0.0f;
        private float gauge = 0.0f;
        private float targetGauge;
        private float startGauge;

        private bool isAnimActive = false;
        private bool isInit = false;

        private const int ICON_LIST_COUNT = 5;

        public void OnReadyGame()
        {
            Display(false);
        }

        public void OnEnterTurn()
        {
            Display(false);
        }

        public void UpdateTotalBet(long totalBetCredit)
        {
            Display(true);
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement sliderElement = ContextUtils.FindElement(rootElement, "Bet Progress Bar", ContextSearchingType.ChildrenSearch);
            contextSlider = sliderElement.GetComponent<ContextSlider>();
            textElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            iconElements = new ContextElement[ICON_LIST_COUNT];
            for (int i = 0; i < ICON_LIST_COUNT; ++i)
            {
                iconElements[i] = ContextUtils.FindElement(rootElement, string.Format("Pack 0{0}", i + 1), ContextSearchingType.ChildrenSearch);
            }

            isInit = true;
        }

        private void Start()
        {
            InitProperty();
        }

        private void Update()
        {
            if (isAnimActive && isInit)
            {
                if (checkTime < displayTime)
                    checkTime += Time.deltaTime;
                else
                    Display(false);

                gaugeTime += Time.deltaTime;
                gauge = startGauge + gaugeTime / waitTime * (targetGauge - startGauge);
                contextSlider.SetFloatProperty(gauge);

                if (gaugeTime >= waitTime)
                {
                    gauge = targetGauge;
                    contextSlider.SetFloatProperty(gauge);
                }

            }
        }

        private void Display(bool isActive)
        {
            isAnimActive = isActive;
            if (isActive)
            {
                checkTime = 0.0f;
                gaugeTime = 0.0f;
                UpdateBalloon();
                UpdateText();
                UpdateProgressBar();
                isAnimActive = !MetaGameUtils.IsMetaGameLevelLocked();
            }

            if(rootAnimator != null)
            {
                rootAnimator.SetBool("IsActive", isAnimActive);
            }
        }

        private void UpdateBalloon()
        {
            float gaugeEnergyAsFloat = BossRaidersUtils.GetCurrentGaugeEnergyAsFloat();

            for (int i = 1; i <= ICON_LIST_COUNT; ++i)
            {
                if (i <= (int)gaugeEnergyAsFloat)
                    UpdatePack(i - 1, true, (i == (int)gaugeEnergyAsFloat) ? gaugeEnergyAsFloat - Math.Truncate(gaugeEnergyAsFloat) : 0);
                else
                    UpdatePack(i - 1, false, 0);
            }
        }

        private void UpdatePack(int index, bool isActive, double scale)
        {
            Color activeColor = Color.white;
            Color inActiveColor = new Color(0.39f, 0.39f, 0.39f);

            Vector3 bigScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 normalScale = new Vector3(0.8f, 0.8f, 0.8f);

            Vector3 chestScale = Vector3.Lerp(normalScale, bigScale, (float)scale);

            iconElements[index].GetComponent<RectTransform>().localScale = chestScale;
            iconElements[index].GetComponent<Image>().color = isActive ? activeColor : inActiveColor;
        }

        private void UpdateText()
        {
            Blackboard bb = BossRaidersUtils.GetEnergyBundleInfoBB(BlackboardUtils.FindVariable<long>("./betCredit").value);
            bool isEligible = (bb != null && bb.GetValue<long>("totalEnergyBundleEarning") > 0L);
            MetaContextElementUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, isEligible ? "BOSS_RAIDERS_IN_GAME_ACTIVE_TEXT" : "BOSS_RAIDERS_IN_GAME_INACTIVE_TEXT"));
        }

        private void UpdateProgressBar()
        {
            targetGauge = BossRaidersUtils.GetCurrentGaugeEnergyAsFloat();
            if (isAnimActive)
                startGauge = gauge;
            else
            {
                contextSlider.SetFloatProperty(targetGauge);
                startGauge = targetGauge;
                gauge = targetGauge;
            }
        }
    }
}
