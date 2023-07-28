using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using ParadoxNotion;
using TMPro;
using System;

namespace GS.Slot.SYC {
    public class SYC_FSGaugeController : MonoBehaviour
    {
        [SerializeField]
        private List<Animator> coinAnimatorPerReel;
        [SerializeField]
        private Animator chrisAnimator;
        [SerializeField]
        private Animator mainAnimator;
        [SerializeField]
        private Animator lightAnimator;
        [SerializeField]
        private Slider gaugeSlider;
        [SerializeField]
        private Animator indicatorAnimator;
        [SerializeField]
        private int indacatorColorChangeCoinCount = 30;
        [SerializeField]
        private List<int> requireCoinCountPerClothOff;
        [SerializeField]
        private float initialGaugeValue = 0.016f;
        [SerializeField]
        private TextMeshProUGUI multiplierText;

        private const string FLY_START_EVENT = "UpdateCoinCountPerReel";
        private const string FLY_END_EVENT = "CoinFlyEnd";
        private const string COIN_COUNT_PER_MULTIPLIER_PATH = "./game/requireCoinCountPerIndex";
        private const string MULTIPLIER_LIST_PATH = "./game/multiplierList";
        private const string COIN_COUNT_FOR_JACKPOT_PATH = "./game/requireCoinCountForJackpot";

        private List<int> coinCountPerMultiplier;
        private List<int> multiplierList;
        private int coinCountForJackpot;

        private int currentCoinCount;
        private int previousMultiplierIndex;
        private int nextChrisClothOffStageIndex;

        private void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentUIDetailEvent", FlyCoinDelegator);
            MessageDispatcher.UnRegister("OnContentUIEvent", OnContentUIEventDelegator);
            StopAllCoroutines();
        }

        private void OnEnable()
        {
            MessageDispatcher.Register("OnContentUIDetailEvent", FlyCoinDelegator);
            MessageDispatcher.Register("OnContentUIEvent", OnContentUIEventDelegator);
            gaugeSlider.maxValue = 1;
            gaugeSlider.minValue = 0;

            currentCoinCount = 0;
            gaugeSlider.value = initialGaugeValue;
            previousMultiplierIndex = 0;
            nextChrisClothOffStageIndex = 0;
            indicatorAnimator.gameObject.SetActive(true);
            coinCountPerMultiplier = BlackboardUtils.FindValue<List<int>>(COIN_COUNT_PER_MULTIPLIER_PATH);
            multiplierList = BlackboardUtils.FindValue<List<int>>(MULTIPLIER_LIST_PATH);
            coinCountForJackpot = BlackboardUtils.FindValue<int>(COIN_COUNT_FOR_JACKPOT_PATH);

            multiplierText.SetText(string.Format("X{0}", 0));
            long betCredit = BlackboardUtils.GetOrCreateVariable<long>("./betCredit").value;
            bool isNotEligible = betCredit < BlackboardUtils.GetOrCreateVariable<List<long>>("./game/jackpotInfo/eligibleMinBetPerJackpot").value[0];
            if (isNotEligible) mainAnimator.SetInteger("Index",0);
            StartCoroutine(CallActionAfterDelay(() =>
            {
                chrisAnimator.SetTrigger("Cloth Off");
            }, 1.5f));
            StartCoroutine(CallActionAfterDelay(() =>
            {
                mainAnimator.SetTrigger("Cloth Off");
            }, 2f));
        }

        private IEnumerator CallActionAfterDelay(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func();
        }

        #region Gauge Logic

        private void FlyCoinDelegator(EventData eventData)
        {
            if (eventData.name != FLY_START_EVENT || !(eventData.value is List<int>)) return;
            FlyCoin(eventData.value as List<int>);
        }

        private void FlyCoin(List<int> coinCountPerReel)
        {
            int totalCointCount = 0;
            for (int i = 0; i < coinCountPerReel.Count; i++)
            {
                int coinCount = coinCountPerReel[i];
                if (coinCount > 0)
                {
                    var animator =  coinAnimatorPerReel[i];
                    animator.gameObject.SetActive(false);
                    animator.gameObject.SetActive(true);
                    animator.SetInteger("Coin Count", coinCount);
                    animator.SetInteger("Reel Index", i);
                    totalCointCount += coinCount;
                }
            }
            GSManager.Instance.GetHandler("Coin Fly").Play();
            mainAnimator.SetTrigger("Collect");
            lightAnimator.SetTrigger("Collect");
            StartCoroutine(CollectCoinAfterSeconds(totalCointCount, 1.4f));
            StartCoroutine(CallActionAfterDelay(() =>
            { GSManager.Instance.GetHandler("Coin Collect").Play(); }
            , 1f));
            MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData(FLY_END_EVENT));
        }

        private IEnumerator CollectCoinAfterSeconds(int coinCount, float delay)
        {
            yield return new WaitForSeconds(delay);
            currentCoinCount += coinCount;
            bool nextMultiplierReached = false;
            if (currentCoinCount >= coinCountForJackpot)
            {
                currentCoinCount = coinCountForJackpot;
                indicatorAnimator.gameObject.SetActive(false);
            }
            else
            {
                int multiplierIndex = 0;
                for (multiplierIndex = 0; multiplierIndex < coinCountPerMultiplier.Count; multiplierIndex++)
                {
                    int requireCoinCount = coinCountPerMultiplier[multiplierIndex];
                    if (currentCoinCount < requireCoinCount)
                    {
                        multiplierIndex--;
                        break;
                    }
                }
                int multiplier = multiplierList[multiplierIndex];
                multiplierText.SetText(string.Format("X{0}", multiplier));

                if (previousMultiplierIndex < multiplierIndex)
                {
                    indicatorAnimator.SetTrigger("Arrive");
                    previousMultiplierIndex = multiplierIndex;
                    nextMultiplierReached = true;

                    string soundName = string.Format("Multiplier Up {0}", multiplierIndex);
                    int voxSoundIndex = multiplierIndex;

                    // if chris is not taking off cloth
                    if (nextChrisClothOffStageIndex >= requireCoinCountPerClothOff.Count
                        || currentCoinCount < requireCoinCountPerClothOff[nextChrisClothOffStageIndex])
                    {
                        if (multiplierIndex >= 6) voxSoundIndex = voxSoundIndex % 6 + 1;
                        string voxSoundName = string.Format("Vox Multiplier Up {0}", voxSoundIndex);
                        GSManager.Instance.GetHandler(voxSoundName).Play();
                    }
                    GSManager.Instance.GetHandler(soundName).Play();
                }
                else indicatorAnimator.SetTrigger("Charge");

                if (currentCoinCount >= indacatorColorChangeCoinCount) indicatorAnimator.SetInteger("Index",1);
            }

            float percentage = (float)currentCoinCount / (float)coinCountForJackpot;
            gaugeSlider.value = percentage;
            GSManager.Instance.GetHandler("Coin Up").Play();

            if (nextChrisClothOffStageIndex < requireCoinCountPerClothOff.Count &&
                    currentCoinCount >= requireCoinCountPerClothOff[nextChrisClothOffStageIndex])
            {
                chrisAnimator.SetTrigger("Cloth Off");
                mainAnimator.SetTrigger("Cloth Off");
                nextChrisClothOffStageIndex++;
                StartCoroutine(CallActionAfterDelay(SendCoinAnimationEndEvent, 4f));
            }
            else if (nextMultiplierReached)
            {
                // if chris only has underwear
                if (nextChrisClothOffStageIndex >= requireCoinCountPerClothOff.Count) chrisAnimator.SetTrigger("Collect");
                StartCoroutine(CallActionAfterDelay(SendCoinAnimationEndEvent, 1.5f));
            }
            else
                SendCoinAnimationEndEvent();

        }

        private void SendCoinAnimationEndEvent()
        {
            MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("CoinAnimationEnd"));
        }
        #endregion

        private const string WIN_ANIM_PARAMETER_NAME = "Win";
        private const string FS_OUTRO_EVENT = "SpinBonusOutro";
        private const string GAUGE_WIN_EVENT = "OnGaugeWin";
        private const string GAUGE_JAKCPOT_WIN_EVENT = "OnGaugeJackpotWin";
        private const string MORE_SPIN_EVENT_NAME = "OnMoreSpin";

        private void OnContentUIEventDelegator(EventData eventData)
        {
            if (eventData.name == FS_OUTRO_EVENT)
            {
                mainAnimator.SetTrigger("Outro");
            }
            else if (eventData.name == GAUGE_WIN_EVENT) {
                mainAnimator.SetTrigger(WIN_ANIM_PARAMETER_NAME);
                lightAnimator.SetTrigger(WIN_ANIM_PARAMETER_NAME);
                chrisAnimator.SetTrigger(WIN_ANIM_PARAMETER_NAME);
                indicatorAnimator.SetTrigger("Multiplier Win");
                GSManager.Instance.GetHandler("Multiplier Complete").Play();
            }
            else if (eventData.name == GAUGE_JAKCPOT_WIN_EVENT)
            {
                mainAnimator.SetTrigger("Grand Jackpot");
                chrisAnimator.SetTrigger(WIN_ANIM_PARAMETER_NAME);
                GSManager.Instance.GetHandler("Multiplier Grand Complete").Play();
            }
            else if (eventData.name == MORE_SPIN_EVENT_NAME)
            {
                chrisAnimator.SetTrigger("More Spin");
            }
        }
    }
}
