using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class ShowDoorView : ActionTask
    {
        protected override void OnExecute()
        {
            var isTrigger = BlackboardUtils.GetOrCreateVariable<bool>(BlackboardUtils.GetContentFSMBlackboard(), "isTriggerMiniGame");
            GameObject door = BlackboardUtils.GetGameContentsBlackboard().GetValue<GameObject>("Door");
            if (isTrigger.value)
            {
                door.gameObject.SetActive(true);
                DoorController doorController = door.GetComponent<DoorController>();
                doorController.OnStart();
            }

            EndAction();
        }
    }
}
