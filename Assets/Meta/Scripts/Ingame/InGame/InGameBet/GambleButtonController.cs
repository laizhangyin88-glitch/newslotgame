using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class GambleButtonController : MonoBehaviour 
    {
        public GameObject gambleButtonArea;

        private const string ON_WIN_EVENT = "OnWinEvent";
        private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

        private MessageDelegates winDelegates;
        private MessageDelegates contentUIDelegates;

        void Awake()
        {
            winDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "TotalWin", OnTotalWin },
                    { "SkipWin",  OnSkipWin  }
                }
            );

            contentUIDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "HideGambleButtonUI", OnHideGambleButtonUI }
                }
            );
        }

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_WIN_EVENT, winDelegates.Delegate);
            MessageDispatcher.Register(ON_CONTENT_UI_EVENT, contentUIDelegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_WIN_EVENT, winDelegates.Delegate);
            MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, contentUIDelegates.Delegate);
        }

        private void OnTotalWin(EventData receivedEvent)
        {
            gambleButtonArea.SetActive(true);
        }

        private void OnSkipWin(EventData receivedEvent)
        {
            if (gambleButtonArea.activeSelf)
                gambleButtonArea.SetActive(false);
        }

        private void OnHideGambleButtonUI(EventData receivedEvent)
        {
            if (gambleButtonArea.activeSelf)
                gambleButtonArea.SetActive(false);
        }
    }

}