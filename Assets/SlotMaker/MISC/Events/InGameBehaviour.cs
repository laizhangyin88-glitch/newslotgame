using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace SlotMaker
{
    public class InGameBehaviour : MonoBehaviour
    {
        public bool showEvents = false;
        public bool enableSystemEvent = true;
        public bool enableContentEvent = true;
        public bool enableSpinButtonEvent = true;

        public UnityEvent onSystemReset;
        public UnityEvent onEnterGame;
        public UnityEvent onExitGame;
        public UnityEvent onLeaveGame;
        public UnityEvent onInitGame;
        public UnityEvent onReadyGame;
        public UnityEvent onEnterTurn;
        public UnityEvent onExitTurn;
        public UnityEvent onFailSpin;
        public UnityEvent onSpinButton;

        private MessageDelegates systemDelegates;
        private MessageDelegates contentDelegates;

        protected virtual void Awake()
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
                        { "BeginGame", BeginGame },
                        { "EndGame",   EndGame   },
                        { "LeaveGame", LeaveGame },
                        { "InitGame",  InitGame  },
                        { "ReadyGame", ReadyGame },
                        { "BeginTurn", BeginTurn },
                        { "EndTurn",   EndTurn   },
                        { "FailSpin",  FailSpin  }
                    }
                );
            }
        }

        protected virtual void OnEnable()
        {
            if (enableSystemEvent)
                MessageDispatcher.Register("OnSystemEvent", systemDelegates.Delegate);

            if (enableContentEvent)
                MessageDispatcher.Register("OnContentEvent", contentDelegates.Delegate);

            if (enableSpinButtonEvent)
                MessageDispatcher.Register("OnSpinButtonEvent", SpinButton);
        }

        protected virtual void OnDisable()
        {
            if (enableSystemEvent)
                MessageDispatcher.UnRegister("OnSystemEvent", systemDelegates.Delegate);

            if (enableContentEvent)
                MessageDispatcher.UnRegister("OnContentEvent", contentDelegates.Delegate);

            if (enableSpinButtonEvent)
                MessageDispatcher.UnRegister("OnSpinButtonEvent", SpinButton);
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

        private void BeginTurn(EventData eventData)
        {
            onEnterTurn.Invoke();
        }

        private void EndTurn(EventData eventData)
        {
            onExitTurn.Invoke();
        }

        private void FailSpin(EventData eventData)
        {
            onFailSpin.Invoke();
        }

        private void SpinButton(EventData eventData)
        {
            onSpinButton.Invoke();
        }
    }
}
