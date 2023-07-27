using System;
using System.Collections;
using UnityEngine;

namespace SlotMaker
{
    public class ContentJackpotCredit : MonoBehaviour
    {
        private IContextText creditText;
        private double fromCredit;
        private double toCredit;
        private float elapsedTime = 0f;
        private float transitionDuration;

        private double multiplier = 1;

        private double currentCredit = 0;

        private long multipliedCurrentCredit
            => GetMultipliedCredit(currentCredit, multiplier);

        public void Reset(
            IContextText creditText,
            string formatString, // Legacy parameter
            long newPrev,
            long newTarget,
            float refreshTime,
            double newMultiplier,
            bool isForce)
        {
            this.creditText = creditText;
            if (isForce && multiplier == newMultiplier)
            {
                // There's no need to refresh
                return;
            }

            double prev;
            double target;

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
                    float rate = elapsedTime/transitionDuration;

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
            }
            else
            {
                // There's no transition needed. Immidiate update and break
                currentCredit = toCredit;
                SetText(multipliedCurrentCredit);
            }

            yield break;
        }

        private long GetMultipliedCredit(double credit, double multiplier)
        {
            return Convert.ToInt64(Convert.ToDouble(credit) * multiplier);
        }

        private void SetText(long credit)
        {
            creditText.SetText(FormatUtility.CommaNumberFormat(credit));
        }
    }
}
