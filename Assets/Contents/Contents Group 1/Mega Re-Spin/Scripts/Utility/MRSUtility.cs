using System;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Audio;

namespace GameStudio.Slot.MRS.Utility
{
    public static class MRSUtility
    {
        static public T TryGetLocalBlackBoardVariable<T>(Blackboard bb, string variableName)
        {
            var bbVariable = BlackboardUtils.FindVariable<T>(bb, variableName);
            if (isValid(bbVariable) == false || (!typeof(T).IsPrimitive && isValid(bbVariable.value) == false))
            {
                return default;
            }
            return bbVariable.value;
        }
        static public T TryGetGlobalBlackBoardVariable<T>(string variableName)
        {
            var bbVariable = BlackboardUtils.FindVariable<T>(variableName);

            if (isValid(bbVariable) == false || (!typeof(T).IsPrimitive && isValid(bbVariable.value) == false))
            {
                return default;
            }

            return bbVariable.value;
        }
        static public void TrySetLocalBlackBoardVariable<T>(Blackboard bb, string variableName, T value)
        {
            var bbVariable = bb.GetVariable<T>(variableName);
            if (isValid(bbVariable) == false || (!typeof(T).IsPrimitive && isValid(bbVariable.value) == false))
            {
                return;
            }
            bbVariable.value = value;
        }
        static public void TrySetGlobalBlackBoardVariable<T>(string variableName, T value)
        {
            var bbVariable = BlackboardUtils.FindVariable<T>(variableName);

            if (isValid(bbVariable) == false || (!typeof(T).IsPrimitive && isValid(bbVariable.value) == false))
            {
                return;
            }
            bbVariable.value = value;
        }

        static public bool isValid<T>(this T variable)
        {
            if (variable == null)
            {
                Debug.LogException(new Exception(MRSLogger.GetLogStackString() + $"is NULL."));
                return false;
            }
            return true;
        }
        static public void ChangeSnapShot(string snapshotName, float timeToReach = 0f)
        {
            AudioMixerSnapshot snapShot = GSManager.Instance.GetAudioMixerSnapshot(snapshotName);
            if (!snapShot.isValid()) throw new Exception($"snapshot string was {snapshotName}");
            snapShot.TransitionTo(timeToReach);

        }
        static public void PlayBonusBGM()
        {
            MessageDispatcher.Dispatch("OnSoundEvent", new EventData("StartBonusBGM"));
        }

        static public void StopBGM()
        {
            MessageDispatcher.Dispatch("OnSoundEvent", new EventData("StopBgm"));
        }
        static public void PlaySound(string soundName)
        {
            IGSHandler sound = GSManager.Instance.GetHandler(soundName);
            if (!sound.isValid()) throw new Exception($"sound string was {soundName}");
            sound.Play();
        }
        static public void StopSound(string soundName)
        {
            IGSHandler sound = GSManager.Instance.GetHandler(soundName);
            if (!sound.isValid()) throw new Exception($"sound string was {soundName}");
            sound.Stop();
        }

        static public void SendEvent<T>(string eventType, string eventName, T eventData)
        {
            MessageDispatcher.Dispatch(eventType, new EventData<T>(eventName, eventData));
        }
        static public void SendEvent(string eventType, string eventName)
        {
            MessageDispatcher.Dispatch(eventType, new EventData(eventName));
        }

        static public Coroutine WaitAllCoroutine(this MonoBehaviour monobehaviour, params Coroutine[] coroutines)
        {
            IEnumerator synthesisMethod()
            { foreach (var each in coroutines) yield return each; }

            return monobehaviour.StartCoroutine(synthesisMethod());
        }

        static public void ExecuteAction<T>(this ActionTask<T> task, T agent) where T : Component => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());
        static public void ExecuteAction(this ActionTask task, Component agent) => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());
    }
}