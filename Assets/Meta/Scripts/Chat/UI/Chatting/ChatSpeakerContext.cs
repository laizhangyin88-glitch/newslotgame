using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChatSpeakerContext : MonoBehaviour
    {
        public ChattingController owner;
        private ContextElement mContextElement => GetComponent<ContextElement>();
        private ContextTextMeshProUGUI textContext;
        private ContextButton plusButtonContext;
        private RemainingTimerController remainingTimer;

        private const string CHAT_GLOBAL_SPEAKER_TEXT = "CHAT_GLOBAL_SPEAKER_TEXT";

        private Variable<int> speakerCount;

        private void Awake()
        {
            speakerCount = BlackboardUtils.FindVariable<int>(null, "/me/speaker");
        }

        private void Start()
        {
            mContextElement.UpdateContext();

            textContext= mContextElement.Find("Text").GetComponent<ContextTextMeshProUGUI>();
            plusButtonContext = mContextElement.Find("Button Plus").GetComponent<ContextButton>();
            
            UpdateSpeaker(null, null);

            plusButtonContext.AddListenerOnClick((context) =>
                {
                    OpenSpeakerPopup();
                }
            );
        }

        private void OnEnable()
        {
            if(speakerCount != null)
                speakerCount.onValueChanged += UpdateSpeaker;
        }

        private void OnDisable()
        {
            if(speakerCount != null)
                speakerCount.onValueChanged -= UpdateSpeaker;
        }

        private void UpdateSpeaker(string name, object value)
        {
            if (speakerCount.value > 0)
            {
                textContext.SetGlobalText(CHAT_GLOBAL_SPEAKER_TEXT, speakerCount.value);
                if (remainingTimer != null)
                {
                    Destroy(remainingTimer);
                }
            }
            else
            {
                if (remainingTimer == null)
                {
                    StartTimer();
                }
            }
        }

        private void StartTimer()
        {
            remainingTimer = textContext.gameObject.AddComponent<RemainingTimerController>();
            remainingTimer.Init(textContext, "TIME_FORMAT_HHMMSS_TOTALHOUR", CHAT_GLOBAL_SPEAKER_TEXT, "", "Ended", false, null);
            remainingTimer.StartTimer(GetNextTimestamp(), 0);
        }

        private long GetNextTimestamp()
        {
            DateTime curDate = TimeUtils.GetLocalDateTime();
            DateTime nextDate = curDate.AddDays(1);
            nextDate = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, 0, 0, 0);
            return (long)(nextDate - TimeUtils.Jan1St1970.ToLocalTime()).TotalMilliseconds;
        }

        public static void OpenSpeakerPopup()
        {
            var popup = MetaObjectUtils.MakeScene("Popup Buy Global Chat Scene", PopupManager.Instance.transform.Find("Area"));
            PopupManager.Instance.Open(popup);
        }
    }
}
