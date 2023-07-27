using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class RemainingTimerController : MonoBehaviour
    {
        public StringTable.StringTableType tableType;

        public IContextText textElement;

        public string timeFormatKey;
        public string outputFormatKey;
        public string warningFormatKey;
        public string expireText;

        public long targetTimeStamp;
        public long warningTimeStamp;

        public bool useCommonTimer;

        public Action callback;
        public bool isStarted { get; private set; }

        public const string TIME_FORMAT_COMMON_TIME_LEFT = "TIME_FORMAT_COMMON_TIME_LEFT";

        public bool IsInit
        {
            get { return textElement != null && timeFormatKey != string.Empty && targetTimeStamp > 0; }
        }

        private void OnEnable()
        {
            if(IsInit)
                StartCoroutine(StartRemainingTimer());
        }

        private void OnDisable()
        {
            StopTimer();
        }

        public void Init(  ContextElement textElement,
                           string timeFormatKey,
                           string outputFormatKey,
                           string warningFormatKey,
                           string expireText,
                           bool useCommonTimer,
                           Action endCallback )
        {
            this.textElement      = textElement as IContextText;
            this.timeFormatKey    = timeFormatKey;
            this.outputFormatKey  = outputFormatKey;
            this.warningFormatKey = warningFormatKey;
            this.expireText       = expireText;
            this.useCommonTimer   = useCommonTimer;
            this.callback         = endCallback;
            targetTimeStamp = 0;
            warningTimeStamp = 0;

            tableType = StringTable.StringTableType.Global;
        }

        public void StartTimer(long targetTimeStamp, long warningTimeStamp)
        {
            isStarted = true;
            this.targetTimeStamp  = targetTimeStamp;
            this.warningTimeStamp = warningTimeStamp;

            StopAllCoroutines();

            if (gameObject.activeInHierarchy && IsInit)
                StartCoroutine(StartRemainingTimer());
        }

        public void StopTimer()
        {
            isStarted = false;
            StopAllCoroutines();
        }

        private IEnumerator StartRemainingTimer()
        {
            DateTime targetDate = TimeUtils.ParseTimestampToDateTime(targetTimeStamp);

            while(true)
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();

                if(currentTimestamp < targetTimeStamp)
                {
                    DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);
                    TimeSpan leftTimespan = targetDate - currentDate;

                    string remainText = null;

                    if(useCommonTimer && leftTimespan.TotalDays >= 1.0)
                    {
                        remainText = StringTableUtils.GetString(tableType, TIME_FORMAT_COMMON_TIME_LEFT, (int)leftTimespan.TotalDays);
                    }
                    else
                    {
                        remainText = StringTableUtils.GetString(tableType, timeFormatKey, leftTimespan.Days, leftTimespan.Hours, leftTimespan.Minutes, leftTimespan.Seconds, Math.Truncate((double)leftTimespan.TotalHours));
                    }

                    if(warningTimeStamp > 0 && currentTimestamp > warningTimeStamp && !string.IsNullOrEmpty(warningFormatKey))
                    {
                        remainText = StringTableUtils.GetString(tableType, warningFormatKey, remainText);
                    }
                    else if(outputFormatKey != null && !string.IsNullOrEmpty(outputFormatKey))
                    {
                        remainText = StringTableUtils.GetString(tableType, outputFormatKey, remainText);
                    }

                    textElement.SetText(remainText);

                    yield return new WaitForSeconds(0.33f);
                }
                else
                {
                    // Set Expire Text.
                    if(!string.IsNullOrEmpty(expireText))
                    {
                        textElement.SetText(expireText);
                    }
                    else
                    {
                        textElement.SetText(StringTableUtils.GetString(tableType, timeFormatKey, 0, 0, 0, 0, 0));
                    }
                    break;
                }
            }

            if(callback != null)
            {
                callback.Invoke();
            }
            isStarted = false;
        }
    }
}
