using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine.Events;

namespace BagelCode
{
    public class MetaIncreaseNumber : MonoBehaviour
    {
        private IContextText creditText;
        private double fromCredit;
        private double toCredit;
        private float elapsedTime = 0f;
        private float transitionDuration;

        private double multiplier = 1;

        private double currentCredit = 0;
        private string formatText = "";
        private UnityAction<long> formatCallback;
        private UnityAction endCallback;

        private long multipliedCurrentCredit => GetMultipliedCredit(currentCredit, multiplier);

        public void Reset(IContextText creditText,
                          string formatString,
                          long newPrev,
                          long newTarget,
                          float refreshTime,
                          double newMultiplier,
                          bool isForce,
                          UnityAction<long> formatAction = null,
                          UnityAction endAction = null)
        {
            this.creditText = creditText;
            if (isForce && multiplier == newMultiplier)
            {
                // There's no need to refresh
                return;
            }

            double prev;
            double target;

            formatText = formatString;
            formatCallback = formatAction;
            endCallback = endAction;

            if (multipliedCurrentCredit == 0)
            {
                // first join
                prev = newPrev;
                target = newTarget;
                elapsedTime = refreshTime;
                InitializeTransitionState(prev, target, newMultiplier, refreshTime);
            }
            else if (!isForce)
            {
                if (toCredit > newTarget)
                {
                    // new target is lower than current set target
                    prev = newPrev;
                }
                else
                {
                    if (currentCredit < newTarget)
                    {
                        // current is set between. Overriding prev
                        prev = currentCredit;
                    }
                    else
                    {
                        // calculating new prev
                        prev = newPrev;
                    }
                }

                target = newTarget;
                InitializeTransitionState(prev, target, newMultiplier, refreshTime);
            }
            else
            {
                multiplier = newMultiplier;
                SetText(multipliedCurrentCredit);
            }
        }

        private void InitializeTransitionState(double fromCredit, double toCredit, double multiplier, float transitionDuration, float elapsedTime = 0f)
        {
            StopAllCoroutines();

            this.fromCredit = fromCredit;
            this.toCredit = toCredit;
            this.transitionDuration = transitionDuration;
            this.elapsedTime = elapsedTime;
            this.multiplier = multiplier;

            StartCoroutine(UpdateJackpotCredit());
        }

        private IEnumerator UpdateJackpotCredit()
        {
            if (fromCredit != toCredit)
            {
                while (elapsedTime < transitionDuration)
                {
                    yield return null;

                    elapsedTime += Time.deltaTime;

                    double nextCredit;
                    float rate = elapsedTime / transitionDuration;

                    if (rate > 1f)
                    {
                        nextCredit = toCredit;
                    }
                    else
                    {
                        double interval = toCredit - fromCredit;
                        nextCredit = fromCredit + (interval * rate);
                    }

                    currentCredit = nextCredit;
                    SetText(multipliedCurrentCredit);
                }

                if (endCallback != null)
                    endCallback();
            }
            else
            {
                // There's no transition needed. Immidiate update and break
                currentCredit = toCredit;
                SetText(multipliedCurrentCredit);
                if (endCallback != null)
                    endCallback();
            }

            yield break;
        }

        private long GetMultipliedCredit(double credit, double multiplier)
        {
            return Convert.ToInt64(Convert.ToDouble(credit) * multiplier);
        }

        private void SetText(long credit)
        {
            if (formatCallback != null)
                formatCallback.Invoke(credit);
            else if (!string.IsNullOrEmpty(formatText))
                creditText.SetGlobalText(formatText, credit);
            else
                creditText.SetText(FormatUtility.CommaNumberFormat(credit));
        }
    }
}