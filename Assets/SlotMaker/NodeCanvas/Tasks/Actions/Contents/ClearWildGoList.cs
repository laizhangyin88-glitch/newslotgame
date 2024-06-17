using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class ClearWildGoList : ActionTask
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            var list = BlackboardUtils.GetOrCreateVariable<List<GameObject>>(BlackboardUtils.GetContentFSMBlackboard(), "_wildList").value;
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject.Destroy(list[i].gameObject);
                }
                list.Clear();
            }
            EndAction();
        }
    }
}
