using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine.Events;
namespace GameStudio.Slot.EDM
{
    public class EDMGameProgressManager : MonoWeakSingleton<EDMGameProgressManager>
    {
        public Blackboard gameProgressBB = null;

        public UnityEvent OnInitalizedGame;
        public UnityEvent OnReceivedResponse;

        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
        private void Dispatch(EventData eventData)
        {
            MessageDispatcher.EventDelegate del;
            if (delegates.TryGetValue(eventData.name, out del))
                del.Invoke(eventData);
        }

        protected void RegisterEvent(string eventName, MessageDispatcher.EventDelegate eventDelegate)
        {
            delegates.Add(eventName, eventDelegate);
        }

        protected void UnRegisterEvent(string eventName)
        {
            delegates.Remove(eventName);
        }

        private void Awake()
        {
            RegisterEvent("EDM_ON_INITALIZED_GAME", OnInitalizeGame);
            RegisterEvent("EDM_ON_RECEIVED_RESPONSE", OnReceivedReponse);

        }

        protected virtual void OnEnable()
        {
            ContentEvent.Register(Dispatch);
        }

        protected virtual void OnDisable()
        {
            ContentEvent.UnRegister(Dispatch);
        }

        private void OnInitalizeGame(EventData eventData)
        {
            Variable<Blackboard> gameProgressVariable = BlackboardUtils.FindVariable<Blackboard>("./game/gameProgress");

            if (gameProgressVariable != null)
                gameProgressBB = gameProgressVariable.value;

            OnInitalizedGame.Invoke();
        }

        private void OnReceivedReponse(EventData eventData)
        {
            Variable<Blackboard> gameProgressVariable = BlackboardUtils.FindVariable<Blackboard>("./spin/response/gameProgress");

            if (gameProgressVariable != null)
                gameProgressBB = gameProgressVariable.value;

            OnReceivedResponse.Invoke();
        }
    }
}