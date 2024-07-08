using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class PlaySoundById : ActionTask
    {
        public BBParameter<string> soundId;

        protected override void OnExecute()
        {
            if (!string.IsNullOrWhiteSpace(soundId.value))
            {
                GSManager.Instance.GetHandler(soundId.value).Play();
            }
            EndAction();
        }
    }
}
