using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SlotMaker;
using System;
using ParadoxNotion;

namespace GS.Slot.SYK
{
    public class SYK_FSMultiplierController : MonoBehaviour
    {
        [SerializeField]
        private Animator mainAnimator;
        [SerializeField]
        private List<Animator> multiplierBoxAnimatorList;
        [SerializeField]
        private List<GameObject> multiplierBoxAnhcorList;
        [SerializeField]
        private List<TextMeshProUGUI> multiplierTextList;
        [SerializeField]
        private GameObject flyingCoin;
        [SerializeField]
        private Transform coinFlyingDest;

        private int indicatingBoxIndex;
        private int accumulatedCascadeWinCount;
        private int cascadeHitCountForMultiplierUp;

        private long nextEmergeMultiplier;
        private long maximumMultiplier;

        #region const string members
        public const string CONTENT_UI_EVENT = "OnContentUIEvent";
        public const string CASCADE_WIN_COUNT_UP_EVENT = "IncreaseCascadeWinCount";
        public const string CASCADE_WIN_COUNT_UP_END_EVENT = "IncreaseCascadeWinCountEnd";
        public const string ON_TOTAL_WIN = "TotalWin";
        public const string ANIMATION_BEFORE_FS_END_EVENT = "OnMultiplierWin";
        public const string MULTIPLIER_UP_EVENT = "OnMultiplierUp";
        public const string ON_WIN_EVENT = "OnWinEvent";

        public const string REQUIRE_HIT_COUNT_FOR_MULTIPLIER_UP_PATH = "./game/cascadeHitCountForMultiplierUp";
        public const string MAX_MULTIPLIER_PATH = "./game/maximumMultiplier";

        public const string MULTIPLIER_TEXT_FORMAT = "X{0}";
        #endregion

        public static IEnumerator CallActionAfterDelay(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func();
        }

        private void OnEnable()
        {
            indicatingBoxIndex = 0;
            accumulatedCascadeWinCount = 0;
            cascadeHitCountForMultiplierUp = BlackboardUtils.FindValue<int>(null, REQUIRE_HIT_COUNT_FOR_MULTIPLIER_UP_PATH);
            maximumMultiplier = BlackboardUtils.FindValue<long>(null, MAX_MULTIPLIER_PATH);
            int index = 0;
            for (; index < multiplierTextList.Count; index++)
            {
                multiplierTextList[index].text = string.Format(MULTIPLIER_TEXT_FORMAT, index + 1);
                multiplierBoxAnhcorList[index].SetActive(true);
                multiplierBoxAnimatorList[index].gameObject.SetActive(true);
            }
            nextEmergeMultiplier = index + 1;

            MessageDispatcher.Register(CONTENT_UI_EVENT, ContentUIDelegator);
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(CONTENT_UI_EVENT, ContentUIDelegator);
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
            StopAllCoroutines();
        }

        private void OnWinEvent(EventData eventData)
        {
            if (eventData.name != ON_TOTAL_WIN) return;
            multiplierBoxAnimatorList[indicatingBoxIndex % multiplierBoxAnimatorList.Count].SetTrigger("Scatter Win");
        }

        private void ContentUIDelegator(EventData eventData)
        {
            if (eventData.name == CASCADE_WIN_COUNT_UP_EVENT) IncreaseCascadeWinCountByOne();
            if (eventData.name == ANIMATION_BEFORE_FS_END_EVENT) OnFSEnd();
        }

        private void OnFSEnd()
        {
            foreach (var animator in multiplierBoxAnimatorList) animator.SetTrigger("Multiplier Win");
        }


        private void IncreaseCascadeWinCountByOne()
        {
            int previousWinCount = accumulatedCascadeWinCount;
            int newWinCount = previousWinCount + 1;
            accumulatedCascadeWinCount = newWinCount;
            bool nextMultiplierReached = newWinCount / cascadeHitCountForMultiplierUp > previousWinCount / cascadeHitCountForMultiplierUp;

            int currentBoxIndex = indicatingBoxIndex % multiplierBoxAnimatorList.Count;
            Animator currentAnimator = multiplierBoxAnimatorList[currentBoxIndex];
            currentAnimator.SetTrigger("Gauge Up");
            if (nextMultiplierReached)
            {
                indicatingBoxIndex++;
                MessageDispatcher.Dispatch(CONTENT_UI_EVENT, new EventData(MULTIPLIER_UP_EVENT));
                StartCoroutine(CallActionAfterDelay(() =>
                    {
                        mainAnimator.SetTrigger("Slide");
                        StartCoroutine(CallActionAfterDelay(SetNextMultiplierBox, 0.2f));
                    }, 2f));

                StartCoroutine(CallActionAfterDelay(FlyCoin, 1f));
            }
            else
            {
                GSManager.Instance.GetHandler("Multiplier Up").Play();
                StartCoroutine(CallActionAfterDelay(() => { MessageDispatcher.Dispatch(CONTENT_UI_EVENT, new EventData(CASCADE_WIN_COUNT_UP_END_EVENT)); }, 1.5f));
            }
        }

        private void FlyCoin()
        {
            // substract 1 becuase "indicatingBoxIndex" is added before it is called
            int currentBoxIndex = (indicatingBoxIndex - 1) % multiplierBoxAnimatorList.Count;
            var positionController = flyingCoin.GetComponentInChildren<DirectionalWeightPositionController>();
            Animator currentAnimator = multiplierBoxAnimatorList[currentBoxIndex];
            positionController.from = currentAnimator.transform;
            positionController.to = coinFlyingDest;
            flyingCoin.SetActive(true);
            GSManager.Instance.GetHandler("Multiplier Coin Land").Play();
            StartCoroutine(CallActionAfterDelay(() =>
            {
                flyingCoin.SetActive(false);
            }, 1.4f));
        }

        private void SetNextMultiplierBox()
        {
            int lastBoxIndex = (indicatingBoxIndex - 1) % multiplierBoxAnimatorList.Count;
            multiplierBoxAnimatorList[lastBoxIndex].gameObject.SetActive(false);
            multiplierBoxAnimatorList[lastBoxIndex].gameObject.SetActive(true);

            TextMeshProUGUI lastText = multiplierTextList[lastBoxIndex];
            lastText.text = string.Format(MULTIPLIER_TEXT_FORMAT, nextEmergeMultiplier);

            bool isOverMaxMultiplier = nextEmergeMultiplier >= maximumMultiplier + 1;
            if (isOverMaxMultiplier)
                multiplierBoxAnhcorList[lastBoxIndex].gameObject.SetActive(false);
             nextEmergeMultiplier = nextEmergeMultiplier + 1;

            StartCoroutine(CallActionAfterDelay(() => { MessageDispatcher.Dispatch(CONTENT_UI_EVENT, new EventData(CASCADE_WIN_COUNT_UP_END_EVENT)); }, 3f));
        }
    }
}