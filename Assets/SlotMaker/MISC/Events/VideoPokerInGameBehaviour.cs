using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace SlotMaker
{
    public class VideoPokerInGameBehaviour : MonoBehaviour 
    {
        public UnityIntEvent onUpdateHandsGroup;
        public UnityIntEvent onUpdateHandsCount;

        private MessageDelegates contentDelegates;

        private void Awake()
        {
            contentDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "UpdateHandsGroup", OnUpdateHandsGroup },
                    { "UpdateHandsCount", OnUpdateHandsCount },
                }
            );
        }

        protected void OnEnable()
        {
            MessageDispatcher.Register("OnContentEvent", contentDelegates.Delegate);
        }

        protected void OnDisable()
        {
            MessageDispatcher.UnRegister("OnContentEvent", contentDelegates.Delegate);
        }

        private void OnUpdateHandsGroup(EventData eventData)
        {
            onUpdateHandsGroup.Invoke(((EventData<int>)eventData).value);
        }

        private void OnUpdateHandsCount(EventData eventData)
        {
            onUpdateHandsCount.Invoke(((EventData<int>)eventData).value);
        }
    }
}