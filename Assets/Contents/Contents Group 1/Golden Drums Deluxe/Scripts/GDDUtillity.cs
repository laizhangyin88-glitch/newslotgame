using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.IoC;
using SlotMaker.Rendering.Clipping2D;
using UnityEngine;
using BagelCode.Slots.GDD;
using UnityEngine.Assertions;
using UnityEngine.Events;
using SlotMaker.Tasks.Actions;
using UnityEngine.Audio;
using System.IO;
using System.Security.Permissions;
using System.Reflection;

namespace BagelCode.Slots.GDD.Utillity
{
    public static class GDDUtillity
    {
        static public T TryGetLocalBlackBoardVariable<T>(Blackboard bb, string variableName)
        {
            var bbVariable = bb.GetVariable<T>(variableName);
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
                Debug.LogException(new Exception(GDDLogger.GetLogStackString() + $"is NULL."));
                return false;
            }
            return true;
        }
        static public void ChangeSnapShot(string snapshotName)
        {
            AudioMixerSnapshot snapShot = GSManager.Instance.GetAudioMixerSnapshot(snapshotName);
            if (!snapShot.isValid()) throw new Exception($"snapshot string was {snapshotName}");
            snapShot.TransitionTo(0);

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

        static public void ExecuteAction<T>(this ActionTask<T> task, T agent) where T : Component => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());
        static public void ExecuteAction(this ActionTask task, Component agent) => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());
    }
}