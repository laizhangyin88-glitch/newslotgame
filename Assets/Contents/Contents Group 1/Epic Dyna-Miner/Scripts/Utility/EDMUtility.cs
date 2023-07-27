using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Audio;

namespace GameStudio.Slot.EDM.Utility
{
    public static class EDMUtility
    {
        public static Vector2Int ConvertCellBBToVector2Int(Blackboard cell)
        {
            int x = cell.GetVariable<int>("x").value;
            int y = cell.GetVariable<int>("y").value;

            return new Vector2Int(x, y);
        }

        public static BaseSymbol GetSymbolFromSpotSlotMachine(BaseSlotMachine slotMachine, Vector2Int cell)
        {
            int x = cell.x;
            int y = cell.y;
            return slotMachine.reels[y * slotMachine.ColumnCount + x].symbols[1];
        }

        static public void ChangeSnapShot(string snapshotName, float timeToReach = 0f)
        {
            AudioMixerSnapshot snapShot = GSManager.Instance.GetAudioMixerSnapshot(snapshotName);
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
            sound.Play();
        }
        static public void StopSound(string soundName)
        {
            IGSHandler sound = GSManager.Instance.GetHandler(soundName);
            sound.Stop();
        }
        static public void ExecuteAction<T>(this ActionTask<T> task, T agent) where T : Component => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());
        static public void ExecuteAction(this ActionTask task, Component agent) => task.ExecuteAction(agent, agent.GetComponent<Blackboard>());

    }
}