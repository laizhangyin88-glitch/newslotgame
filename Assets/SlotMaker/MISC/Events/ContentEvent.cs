using ParadoxNotion;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public enum ContentNodeType
    {
        Turn,
        Spin,
        Bonus
    };

    public static class ContentEvent
    {
        public static readonly string ON_CONTENT_EVENT = "OnContentEvent";
        public static readonly string ON_BEGIN_GAME_EVENT = "BeginGame";
        public static readonly string ON_END_GAME_EVENT = "EndGame";
        public static readonly string ON_INIT_GAME = "InitGame";
        public static readonly string ON_READY_GAME = "ReadyGame";
        public static readonly string ON_BEGIN_TURN_EVENT = "BeginTurn";
        public static readonly string ON_END_TURN_EVENT = "EndTurn";
        public static readonly string ON_BEGIN_SPIN_EVENT = "BeginSpin";
        public static readonly string ON_END_SPIN_EVENT = "EndSpin";
        public static readonly string ON_BEGIN_BONUS_EVENT = "BeginBonus";
        public static readonly string ON_END_BONUS_EVENT = "EndBonus";

        public static void BeginGame(Blackboard game)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_BEGIN_GAME_EVENT, game));
        }

        public static void EndGame(Blackboard game)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_END_GAME_EVENT, game));
        }

        public static void BeginTurn(Blackboard turn)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_BEGIN_TURN_EVENT, turn));
        }

        public static void EndTurn(Blackboard turn)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_END_TURN_EVENT, turn));
        }

        public static void BeginSpin(Blackboard spin)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_BEGIN_SPIN_EVENT, spin));
        }

        public static void EndSpin(Blackboard spin)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_END_SPIN_EVENT, spin));
        }

        public static void BeginBonus(Blackboard bonus)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_BEGIN_BONUS_EVENT, bonus));
        }

        public static void EndBonus(Blackboard bonus)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<Blackboard>(ON_END_BONUS_EVENT, bonus));
        }

        public static void Register(MessageDispatcher.EventDelegate del)
        {
            MessageDispatcher.Register(ON_CONTENT_EVENT, del);
        }

        public static void UnRegister(MessageDispatcher.EventDelegate del)
        {
            MessageDispatcher.UnRegister(ON_CONTENT_EVENT, del);
        }

        public static void SendEvent(string eventName)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData(eventName));
        }

        public static void SendEvent<T>(string eventName, T eventData)
        {
            MessageDispatcher.Dispatch(ON_CONTENT_EVENT, new EventData<T>(eventName, eventData));
        }
    }
}
