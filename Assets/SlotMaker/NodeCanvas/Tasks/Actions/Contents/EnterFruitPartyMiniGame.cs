using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class EnterFruitPartyMiniGame : ActionTask
    {
        public BBParameter<bool> isEnter;

        protected override void OnExecute()
        {
            base.OnExecute();
            if(isEnter != null && isEnter.value)
            {
                Debug.LogError("Enter..................................");
                MiniGameDataManagers.Instance.EnterMiniGame();
            }
            else
            {
                Debug.LogError("Exit..................................");
                MiniGameDataManagers.Instance.ExitMiniGame();
            }
            EndAction();
        }
    }
}
