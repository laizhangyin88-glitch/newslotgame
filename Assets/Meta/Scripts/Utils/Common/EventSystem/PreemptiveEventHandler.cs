using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    public class PreemptiveEventHandler
    {
        public PreemptiveEventHandler(MonoBehaviour _agent)
        {
            agent = _agent;

            EnableHandler();
        }

        ~PreemptiveEventHandler()
        {
            DisableHandler();
        }

        private MonoBehaviour agent;

        private Coroutine handleTriggerCoroutine;

        private Dictionary<Trigger, object> triggerActionDict = new Dictionary<Trigger, object>();

        //

        public void InitHandler()
        {
            ClearEvents();
            DisableHandler();
            EnableHandler();
        }

        public void ClearEvents()
        {
            triggerActionDict.Clear();
        }

        public void EnableHandler()
        {
            if (handleTriggerCoroutine == null)
            {
                handleTriggerCoroutine = agent.StartCoroutine(HandleTriggerCoroutine());
            }
        }

        public void DisableHandler()
        {
            if (handleTriggerCoroutine != null)
            {
                agent.StopCoroutine(handleTriggerCoroutine);
                handleTriggerCoroutine = null;

                // Reset All Trigger
                foreach(var trigger in triggerActionDict.Keys)
                {
                    trigger.Reset();
                }
            }
        }

        // the trigger will be ignored when the other action(that registered by this function) is running
        public void Register(Trigger trigger, System.Func<IEnumerator> action)
        {
            triggerActionDict.Add(trigger, action);
        }

        public void Register(Trigger trigger, System.Func<Trigger, IEnumerator> action)
        {
            triggerActionDict.Add(trigger, action);
        }

        public void Register(Trigger trigger, System.Action action)
        {
            triggerActionDict.Add(trigger, action);
        }

        public void Register(Trigger trigger, System.Action<Trigger> action)
        {
            triggerActionDict.Add(trigger, action);
        }

        public bool UnRegister(Trigger trigger)
        {
            return triggerActionDict.Remove(trigger);
        }

        private IEnumerator HandleTriggerCoroutine()
        {
            Trigger triggeredTrigger;

            do
            {
                if (agent.enabled)
                {
                    triggeredTrigger = null;
                    foreach (var trigger in triggerActionDict.Keys)
                    {
                        // WaitUntilTrigger 로 대기하는 trigger 사용 시 Update 가 프레임 내에 두 번 이상 호출될 수 있음
                        // 해당 상황 발생 시 Update 가 프레임 내에 한 번 까지만 호출될 수 있도록 수정 필요
                        trigger.Update();

                        if (trigger.IsTrigger)
                        {
                            triggeredTrigger = trigger;
                            break;
                        }
                    }

                    if (triggeredTrigger != null)
                    {
                        var action = triggerActionDict[triggeredTrigger];

                        if(action is System.Action _action)
                        {
                            _action?.Invoke();
                        }
                        else if(action is System.Action<Trigger> _action2)
                        {
                            _action2?.Invoke(triggeredTrigger);
                        }
                        else if (action is System.Func<IEnumerator> _ienumerator)
                        {
                            yield return agent.StartCoroutine(_ienumerator?.Invoke());
                        }
                        else if (action is System.Func<Trigger, IEnumerator> _ienumerator2)
                        {
                            yield return agent.StartCoroutine(_ienumerator2?.Invoke(triggeredTrigger));
                        }

                        triggeredTrigger.Reset();
                    }
                }

                yield return new WaitForEndOfFrame();

            } while (true);
        }
    }
}
