using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Friend")]
public class SetFriendSuggestList : ActionTask<Blackboard>
{
    public BBParameter<string> encourageList;
    public BBParameter<ContextElement> layout;
    public BBParameter<List<ContextElement>> saveAs;

    protected override string info
    {
        get { return string.Format("Set {0} to {1}", encourageList, layout); }
    }

    protected override void OnExecute()
    {
        var suggestList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, encourageList.value);

        if (suggestList == null || suggestList.value == null)
        {
            EndAction();
            return;
        }

        List<ContextElement> cellList = new List<ContextElement>();
        for (int i = 0; i < layout.value.transform.childCount; ++i)
        {
            ContextElement element = layout.value.transform.GetChild(i).GetComponent<ContextElement>();

            if(suggestList.value.Count > i)
            {
                var bb = element.transform.GetComponent<Blackboard>();
                if (bb != null)
                {
                    var caller = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, "caller");
                    caller.value = agent.gameObject;

                    var info = BlackboardUtils.GetOrCreateVariable<Blackboard>(bb, "info");
                    info.value = suggestList.value[i];
            
                    // var e = new ParadoxNotion.EventData("UpdateCell");
                    // element.transform.GetComponent<NodeCanvas.Framework.GraphOwner>().SendEvent(e);    
                }
            }
            else
            {
                element.gameObject.SetActive(false);
                // cellList.Add(element);
            }
        }

        EndAction();
    }
}

}
