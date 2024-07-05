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
            GameObject door = BlackboardUtils.GetGameContentsBlackboard().GetValue<GameObject>("Door"); 
            door.gameObject.SetActive(true);
            DoorController doorController = door.GetComponent<DoorController>();
            doorController.OnStart();
            EndAction();
        }
    }
}
