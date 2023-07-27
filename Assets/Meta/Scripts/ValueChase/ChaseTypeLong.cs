using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using SlotMaker;

namespace BagelCode
{

    public class ChaseTypeLong : MonoBehaviour
    {
        public ContextElement textElement;

        public long  prev;
        public long  progress;
        public long  current;
        public int   deltaMS;
        public float interTime;
        public long showMultiplierNumerator;

        public string key;
        public StringTable.StringTableType tableType;
        public IContextText property;

        private long amountCoins = 0;
        private long amountSecCoins = 0;

        private bool isWait = true;
        private bool isNonStop = false;

        private Variable<long> updatedProgress;

        public void SetNonstopChase(ContextElement targetText,
                                    long newPrev,
                                    long newCurrent,
                                    int newDeltaMS,
                                    string newStringKey,
                                    StringTable.StringTableType newTableType,
                                    bool isNonStopValue,
                                    long newShowMultiplierNumerator,
                                    Variable<long> newUpdatedProgress = null)
        {
            textElement = targetText;
            prev = newPrev;
            current = newCurrent;
            deltaMS = newDeltaMS;
            key = newStringKey;
            tableType = newTableType;
            isNonStop = isNonStopValue;
            showMultiplierNumerator = newShowMultiplierNumerator;
            updatedProgress = newUpdatedProgress;

            ExecuteChase();
        }

        private void ExecuteChase()
        {
            if(textElement != null)
            {
                property  = textElement as IContextText;

                ApplyProgress();
                interTime = 0f;

                if (prev >= current)
                {
                    StopAllCoroutines();
                    SetText(current);
                    isWait = true;
                }
                else
                {
                    amountCoins = current - prev;
                    amountSecCoins = (long)((double)amountCoins / ((double)deltaMS/1000.0));

                    if(isWait && gameObject.activeInHierarchy)
                    {
                        StartCoroutine(UpdateLong());
                    }
                }
            }
        }

        private void OnEnable()
        {
            ExecuteChase();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            UpdateProgress();

            isWait = true;
        }

        private void UpdateProgress()
        {
            if(updatedProgress != null)
            {
                updatedProgress.value = progress;
            }
        }

        private void ApplyProgress()
        {
            if(updatedProgress != null)
            {
                progress = updatedProgress.value;
            }

            if (progress > current)
            {
                progress = 0L;
                UpdateProgress();
            }
            else if (progress != 0L)
            {
                prev = progress;
            }
        }

        private void SetText(long credit)
        {
            if(property == null) return;

            if(showMultiplierNumerator > NumberUtils.GetGlobalDenominator())
                credit = NumberUtils.GetMultiplierNumeratorValue(credit, showMultiplierNumerator);

            if(string.IsNullOrEmpty(key))
            {
                property.SetText(FormatUtility.CommaNumberFormat(credit));
            }
            else
            {
                bool error = true;
                property.SetText(StringTableUtils.GetString(tableType, key, credit, out error));
            }
        }

        private bool UpdateCurrentCoins()
        {
            interTime += Time.deltaTime;

            long last = progress;

            progress = prev + (long)((double)amountSecCoins * (double)interTime);
            if(!isNonStop && progress >= current)
                progress = current;

            return progress != last;
        }

        private IEnumerator UpdateLong()
        {
            isWait = false;

            while(true)
            {
                if (UpdateCurrentCoins())
                {
                    SetText(progress);
                    UpdateProgress();
                }

                if(!isNonStop && progress >= current)
                    break;
                else if (prev > progress)
                    break;

                yield return null;
            }

            SetText(current);

            isWait = true;
        }
    }

}
