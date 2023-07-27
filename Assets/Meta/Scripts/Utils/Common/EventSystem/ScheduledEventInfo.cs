using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public class ScheduledEventInfo
    {
        public Coroutine coroutine = null;

        private MonoBehaviour agent;
        private object action;
        private float delay;
        private bool repeat;
        private bool dispatchOnRegister;

        private float remain;

        private bool triggered = false;

        public ScheduledEventInfo(MonoBehaviour _agent, System.Func<IEnumerator> _action, float _delay, bool _repeat, bool _dispatchOnRegister)
        {
            agent = _agent;
            action = _action;
            delay = _delay;
            repeat = _repeat;
            dispatchOnRegister = _dispatchOnRegister;
            remain = dispatchOnRegister ? 0f : delay;
        }

        public ScheduledEventInfo(MonoBehaviour _agent, System.Action _action, float _delay, bool _repeat, bool _dispatchOnRegister)
        {
            agent = _agent;
            action = _action;
            delay = _delay;
            repeat = _repeat;
            dispatchOnRegister = _dispatchOnRegister;
            remain = dispatchOnRegister ? 0f : delay;
        }

        public bool Update()
        {
            if (triggered && !repeat) return false;

            if (remain <= 0f)
            {
                remain = delay;
                return true;
            }
            else
            {
                remain -= Time.deltaTime;
                return false;
            }
        }

        public IEnumerator OnTrigger()
        {
            triggered = true;

            if (action is System.Action _action)
            {
                _action?.Invoke();
                yield break;
            }
            else if(action is System.Func<IEnumerator> _action2)
            {
                yield return agent.StartCoroutine(_action2?.Invoke());
            }
        }
    }
}