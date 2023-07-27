using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGameInGameGaugeLevel : ActionTask<ContextElement>
    {
        public BBParameter<bool> updateWhileActive;
        public BBParameter<float> gauge;

        private float targetGauge;
        private float startGauge;
        private float waitTime = 0.1f;
        private ContextElement sliderElement;
        
        private Animator packAnimator;
        private Animator iconAnimator;
        
        protected override string info
        {
            get{ return "Update Collecting Game In Game Gauge Level"; }
        }

        protected override void OnExecute ()
        {
            UpdateBalloon();
            UpdateProgressBar();
        }

        protected override void OnUpdate()
        {
            gauge.value = startGauge + elapsedTime / waitTime * (targetGauge - startGauge);
            sliderElement.GetComponent<ContextSlider>().SetFloatProperty(gauge.value);

            if (elapsedTime >= waitTime)
            {
                gauge.value = targetGauge;
                sliderElement.GetComponent<ContextSlider>().SetFloatProperty(gauge.value);
                EndAction();
            }
        }

        private void UpdateProgressBar()
        {
            sliderElement = ContextUtils.FindElement(agent, "Collecting Game Bet Progress/Bet Progress Bar", ContextSearchingType.FullNameSearch);
            targetGauge = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();

            if (updateWhileActive.value)
            {
                startGauge = gauge.value;
            }
            else
            {
                sliderElement.GetComponent<ContextSlider>().SetFloatProperty(targetGauge);
                startGauge = targetGauge;
                gauge.value = targetGauge;
                EndAction();
            }
        }
        
        private void UpdateBalloon()
        {
            ContextElement collectingGameIconElement = ContextUtils.FindElement(agent, "Collecting Game Button/Icon Area/Collecting Game Icon", ContextSearchingType.FullNameSearch);
            iconAnimator = collectingGameIconElement.GetComponent<Animator>();
            packAnimator = ContextUtils.FindElement(agent, "Collecting Game Bet Progress", ContextSearchingType.ChildrenSearch).GetComponent<Animator>();
            packAnimator.SetBool("IsActive", true);

            float gaugeLevelAsFloat = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();

            for (int i = 1; i <= 4; i++)
            {
                if (i <= (int) gaugeLevelAsFloat)
                {
                    if (i == (int) gaugeLevelAsFloat)
                        UpdatePack(i, true, gaugeLevelAsFloat - Math.Truncate(gaugeLevelAsFloat));
                    else
                        UpdatePack(i, true, 0);
                }
                else
                {
                    UpdatePack(i, false, 0);
                }
            }

            Debug.LogError(gaugeLevelAsFloat);
            iconAnimator.SetBool("IsLocked", gaugeLevelAsFloat < 1f);

            ContextElement textElement = ContextUtils.FindElement(agent, "Collecting Game Bet Progress/Text", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(textElement, gaugeLevelAsFloat < 1f
                ? StringTableUtils.GetString(StringTable.StringTableType.Global, "COLLECTING_GAME_IN_GAME_INACTIVE_TEXT")
                : StringTableUtils.GetString(StringTable.StringTableType.Global, "COLLECTING_GAME_IN_GAME_ACTIVE_TEXT"));
        }
        
        private void UpdatePack(int index, bool isActive, double scale)
        {
            Color activeColor = Color.white;
            Color inActiveColor = new Color(0.39f, 0.39f, 0.39f);

            Vector3 bigScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 normalScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            Vector3 chestScale = Vector3.Lerp(normalScale, bigScale, (float) scale);
            
            ContextElement packElement = ContextUtils.FindElement(agent, string.Format("Collecting Game Bet Progress/Pack 0{0}", index), ContextSearchingType.FullNameSearch);
            packElement.GetComponent<RectTransform>().localScale = chestScale;
            packElement.GetComponent<Image>().color = isActive ? activeColor : inActiveColor;
        }
    }
}
