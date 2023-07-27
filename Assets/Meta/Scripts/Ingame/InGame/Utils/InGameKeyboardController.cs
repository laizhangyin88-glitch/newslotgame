using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class InGameKeyboardController : MonoBehaviour
    {
        private bool activateChatting;

        private SceneState currentSceneState
        {
            get { return BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "currentSceneState")?.value ?? SceneState.MAX; }
        }

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ACTIVATE_CHATTING_EVENT = "OnChatToggle";
        private const string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";
        private const string ON_CLICK_CHAT = "OnClickChat";
        
        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name.Equals(ACTIVATE_CHATTING_EVENT, StringComparison.Ordinal))
                activateChatting = ((EventData<bool>)eventData).value;
        }

        private void Update()
        {
            if (!activateChatting)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if(PopupManager.Instance.popupCount == 0 && currentSceneState == SceneState.INGAME)
                        MessageDispatcher.Dispatch(ON_SPINBUTTON_EVENT, new EventData(ON_SPINBUTTON_EVENT));
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    EventSender.SendGlobalEvent(ON_CLICK_CHAT);
                }
            }
        }
    }
}
