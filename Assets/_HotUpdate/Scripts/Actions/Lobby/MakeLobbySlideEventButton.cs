using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Lobby")]

public class MakeLobbySlideEventButton : ActionTask <Blackboard> 
{
    public BBParameter<GameObject> emptyObj;

    public BBParameter<GameObject> saveAsButtonObj;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private ContextElement scrollContentsElement;

    private bool isInit = false;

    protected override string info
    { 
        get 
        { 
            return string.Format("Make Slide Event Button");
        } 
    }

    protected override void OnExecute()
    {
        if(saveAsButtonObj.value != null)
            Object.Destroy(saveAsButtonObj.value);
            
        EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();

        if(eventInfo != null)
        {
            Init();

            // Event. 
            ContextElement metaEventButtonElement = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Lobby Slide Inside Button Event", scrollContentsElement.transform, "", "Meta Event Button").GetComponent<ContextElement>();
            saveAsButtonObj.value = metaEventButtonElement.gameObject;

            int sibilingIndex = emptyObj.value.transform.GetSiblingIndex();
            saveAsButtonObj.value.transform.SetSiblingIndex(sibilingIndex);
        }

        EndAction();
    }

    private void Init()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        scrollContentsElement = ContextUtils.FindElement(agentElement, "Lobby Slide Inside/Contents", ContextSearchingType.FullNameSearch);

        isInit = true;
    }
}

}
