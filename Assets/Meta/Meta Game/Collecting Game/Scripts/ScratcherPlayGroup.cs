using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.Scratcher
{
    public abstract class ScratcherPlayGroup
    {
        public bool isPlayNextInstantly = false;
        public float instantOpenDelay = 0f;

        public bool isLast = false;

        protected string eventName = "";

        public enum EventType
        {
            NONE = 0,
            PLAY_NEXT = 1,
            PLAY_NEXT_INSTANT = 2,
            FINISH_SCRATCHER = 3,
        }

        public bool IsEventNameNullorEmpty()
        {
            return string.IsNullOrEmpty(eventName);
        }

        public void SetEventName(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.PLAY_NEXT:
                    eventName = "OnGroupPlayFinished";
                    break;
                case EventType.FINISH_SCRATCHER:
                    eventName = "OnAllGroupPlayFinished";
                    break;
                case EventType.PLAY_NEXT_INSTANT:
                    eventName = "PlayNextInstant";
                    break;
                case EventType.NONE:
                    eventName = "DoNothing";
                    break;
            }
        }

        public abstract IEnumerator Play(ScratcherPlayController controller);

        public void SendEvent(float delay, ScratcherPlayController controller)
        {
            if (string.IsNullOrEmpty(eventName)) return;

            var bb = controller.GetComponent<Blackboard>();
            bb.AddVariable("_openDelay", delay);
            controller.GetComponent<GraphOwner>().SendEvent(eventName);
        }
    }
}