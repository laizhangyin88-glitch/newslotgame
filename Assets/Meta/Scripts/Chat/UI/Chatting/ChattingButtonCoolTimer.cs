using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;
using ParadoxNotion;

namespace BagelCode.Chat
{
    public class ChattingButtonCoolTimer : MonoBehaviour
    {
        public ChattingButtonArea owner;

        public CanvasGroup canvasGroup;

        private Transform buttonsArea;

        private float unLockTime = -1f;
        private float coolTime = 0f;
        private bool isInit = false;

        private const string POST_CHAT_MESSAGE = "OnPostChatMessage";

        private Dictionary<string, MessageDispatcher.EventDelegate> metaUIDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private void Awake()
        {
            buttonsArea = transform.Find("Layout/Button Group");
            canvasGroup = buttonsArea.gameObject.AddComponent<CanvasGroup>();

            metaUIDelegates[POST_CHAT_MESSAGE] = OnPostChatMessage;

            coolTime = ((float)BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "instantMessageCooltime").value) / 1000f;
        }

        private void Start()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);

            isInit = true;
        }

        private void OnEnable()
        {
            if(isInit)
                RefreshInteractable();
        }

        private void OnDisable()
        {
            if(isInit)
                CancelInvoke("RefreshInteractable");
        }

        private void OnDestroy()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (metaUIDelegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        private void RefreshInteractable()
        {
            canvasGroup.interactable = unLockTime < Time.realtimeSinceStartup;
            if(!canvasGroup.interactable)
            {
                if(gameObject.activeInHierarchy)
                {
                    Invoke("RefreshInteractable", unLockTime - Time.realtimeSinceStartup);
                }
            }
            else
            {
                owner.UnityBugFunc();
            }
        }

        private void OnPostChatMessage(EventData eventData)
        {
            if(coolTime <= 0f) return;

            unLockTime = ((float)eventData.value) + coolTime;
            RefreshInteractable();
        }
    }
}