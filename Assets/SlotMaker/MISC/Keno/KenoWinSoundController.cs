using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker.Keno
{
    public class KenoWinSoundController : MonoBehaviour
    {
        [Serializable]
        public class CreditSound
        {
            public string creditSound;
            public string creditEndSound;
            public long maximumCreditRate;
        }
        public List<CreditSound> creditSounds;

        private int grade = -1;
        private bool isPlaying;

        private MessageDelegates creditDelegates;

        private const string BET_CREDIT_PATH = "./turn/totalBetCredit";
        private const string SINGLE_CREDIT_PATH = "./current/singleCredit";

        protected void Awake()
        {
            creditDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "UpdateTurnCredit",   UpdateTurnCredit   },
                    { "UpdatedTurnCredit",  UpdatedTurnCredit  }
                }
            );
        }

        protected void OnEnable()
        {
            MessageDispatcher.Register("OnCreditEvent", creditDelegates.Delegate);
        }

        protected void OnDisable()
        {
            MessageDispatcher.UnRegister("OnCreditEvent", creditDelegates.Delegate);
        }

        private void UpdateTurnCredit(EventData eventData)
        {
            if ((bool)eventData.value)
                Stop();
        }

        private void UpdatedTurnCredit(EventData eventData)
        {
            Stop();
        }

        public void Play()
        {
            if (isPlaying) return;

            grade = -1;
            for (int i = 0, count = creditSounds.Count; i < count; ++i)
            {
                // TODO : Check maximumCreditRate

                GSManager.Instance.GetHandler(creditSounds[i].creditSound).Play();
                grade = i;
                isPlaying = true;
                break;
            }
        }

        private void Stop()
        {
            if (grade >= 0)
            {
                if (!string.IsNullOrEmpty(creditSounds[grade].creditEndSound))
                {
                    GSManager.Instance.GetHandler(creditSounds[grade].creditSound).Stop();
                    GSManager.Instance.GetHandler(creditSounds[grade].creditEndSound).Play();
                }
                grade = -1;
            }
            
            isPlaying = false;
        }
    }
}
