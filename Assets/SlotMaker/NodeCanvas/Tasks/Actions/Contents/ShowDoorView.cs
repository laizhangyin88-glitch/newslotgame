using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class ShowDoorView : ActionTask
    {
        public BBParameter<string> key;

        protected override void OnExecute()
        {
            var spin = ContentBlackboard.Get();
            if (spin != null)
            {
                bool isTrigger = spin.GetValue<bool>(key.value); 
                GameObject door = BlackboardUtils.GetGameContentsBlackboard().GetValue<GameObject>("Door");
                if (isTrigger)
                {
                    door.gameObject.SetActive(true);
                    DoorController doorController = door.GetComponent<DoorController>();
                    doorController.OnStart();
                }
            }
            EndAction();
        }
    }
}
