using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.Keno;

namespace SlotMaker.Keno.Commands
{
    public enum Status
    {
        Failure = 0,
        Success = 1,
        Running = 2,
        Resting = 3,
    }

    [Serializable]
    public abstract class Command : ScriptableObject
    {
        protected float startedTime = 0;
        protected float elapsedTime {
            get
            {
                if (status == Status.Running)
                    return Time.time - startedTime;
                return 0;
            }
        }
        protected Status status = Status.Resting;

        protected void EndAction() { EndAction(true); }
        protected void EndAction(bool success)
        {
            if (status != Status.Running)
                return;

            status = success == true ? Status.Success : Status.Failure;
        }
    }

    public abstract class Command<EventData, Comp> : Command
        where EventData: class
        where Comp: Component
    {
        protected Comp agent;
        protected Comp GetComp(GameObject gameObject) { return gameObject.GetComponent((typeof(Comp))) as Comp; }
        public Comp SetComp(GameObject gameObject) { return agent = GetComp(gameObject); }

        public bool fixedUpdate = false;

        IEnumerator CommandUpdater(EventData eventData) {
            while (ExecuteCommand(eventData) == Status.Running)
            {
                if (!fixedUpdate)
                    yield return new WaitForEndOfFrame();
                else
                    yield return new WaitForFixedUpdate();
            }
            Reset();
        }

        public void Reset() { status = Status.Resting; }

        public void ExecuteCommand(EventData eventData, GameObject gameObject)
        {
            if (SetComp(gameObject) == null)
                return;

            gameObject.GetComponent<MonoBehaviour>().StartCoroutine(CommandUpdater(eventData));
        }
        public Status ExecuteCommand(EventData eventData)
        {
            if (status != Status.Running)
            {
                startedTime = Time.time;
                status = Status.Running;
                OnExecute(eventData);
            }

            if (status == Status.Running)
                OnUpdate(eventData);

            return status;
        }

        protected virtual void OnUpdate(EventData eventData) {}
        protected virtual void OnExecute(EventData eventData) {}
    }

    // TODO
    public class CommandList<EventData, Comp> : Command where EventData: class where Comp: Component {}
}
