using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace SlotMaker
{
    public class VideoPokerBehaviour : MonoBehaviour 
    {
        public UnityEvent onReady;
        public UnityEvent onDeal;
        public UnityEvent onHolding;
        public UnityEvent onDrawing;
        public UnityEvent onDraw;

        public UnityEvent onSystemReset;
        public UnityEvent onEnterGame;
        public UnityEvent onExitGame;
        public UnityEvent onLeaveGame;
        public UnityEvent onInitGame;
        public UnityEvent onReadyGame;

        public UnityIntEvent onUpdateHandsCount;

        public bool enableSystemEvent = true;
        public bool enableContentEvent = true;

        private MessageDelegates systemDelegates;
        private MessageDelegates contentDelegates;

        private void Awake()
        {
            if (enableSystemEvent)
            {
                systemDelegates = new MessageDelegates
                (
                    new Dictionary<string, MessageDispatcher.EventDelegate>
                    {
                        { "SystemReset", SystemReset }
                    }
                );
            }

            if (enableContentEvent)
            {
                contentDelegates = new MessageDelegates
                (
                    new Dictionary<string, MessageDispatcher.EventDelegate>
                    {
                        { "Ready",            Ready            },
                        { "Deal",             Deal             },
                        { "Holding",          Holding          },
                        { "Drawing",          Drawing          },
                        { "Draw",             Draw             },
                        { "UpdateHandsCount", UpdateHandsCount },

                        { "BeginGame",        BeginGame },
                        { "EndGame",          EndGame   },
                        { "LeaveGame",        LeaveGame },
                        { "InitGame",         InitGame  },
                        { "ReadyGame",        ReadyGame },
                    }
                );
            }
        }

        private void OnEnable()
        {
            if (enableSystemEvent)
                MessageDispatcher.Register("OnSystemEvent", systemDelegates.Delegate);

            if (enableContentEvent)
                MessageDispatcher.Register("OnContentEvent", contentDelegates.Delegate);
        }

        private void OnDisable()
        {
            if (enableSystemEvent)
                MessageDispatcher.UnRegister("OnSystemEvent", systemDelegates.Delegate);

            if (enableContentEvent)                
                MessageDispatcher.UnRegister("OnContentEvent", contentDelegates.Delegate);   
        }

        private void Ready(EventData eventData)
        {
            onReady.Invoke();
        }

        private void Deal(EventData eventData)
        {
            onDeal.Invoke();
        }

        private void Draw(EventData eventData)
        {
            onDraw.Invoke();
        }

        private void Holding(EventData eventData)
        {
            onHolding.Invoke();
        }

        private void Drawing(EventData eventData)
        {
            onDrawing.Invoke();
        }

        private void SystemReset(EventData eventData)
        {
            onSystemReset.Invoke();
        }

        private void BeginGame(EventData eventData)
        {
            onEnterGame.Invoke();
        }

        private void EndGame(EventData eventData)
        {
            onExitGame.Invoke();
        }

        private void LeaveGame(EventData eventData)
        {
            onLeaveGame.Invoke();
        }

        private void InitGame(EventData eventData)
        {
            onInitGame.Invoke();
        }

        private void ReadyGame(EventData eventData)
        {
            onReadyGame.Invoke();
        }

        private void UpdateHandsCount(EventData eventData)
        {
            int handsCount = ((EventData<int>)eventData).value;
            onUpdateHandsCount.Invoke(handsCount);
        }
    }
}
