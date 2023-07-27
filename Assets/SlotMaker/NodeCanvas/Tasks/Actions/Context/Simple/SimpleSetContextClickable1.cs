using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextClickable1 : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<string> key;
    public BBParameter<string> arg1;
    public BBParameter<string> eventName;
    public BBParameter<bool>   ignoreReset;
    public bool sendGlobal;

    private const string TEXT_ELEMENT_NAME = "Text";

    protected override string info
    {
        get
        {
            if(ignoreReset.value == true)
            {
                return string.Format("(Additive) {0}.onClick += {1}{2}", agentInfo, (sendGlobal ? "(Global)" : ""), eventName);
            }

            return string.Format("(Reset) {0}.onClick = {1}{2}", agentInfo, (sendGlobal ? "(Global)" : ""), eventName);
        }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        IContextText textElement = element.Find(TEXT_ELEMENT_NAME) as IContextText;
        bool error = true;
        string _eventName = eventName.value;

        Blackboard bb = agent.GetComponent<Blackboard>();
        object a1 = BlackboardUtils.FindValue(bb, arg1.value);
        if (a1 == null)
        {
            EndAction(false);
        }

        if (textElement != null && !string.IsNullOrEmpty(key.value))
        {
            textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, key.value, a1, out error));
        }

        IContextClickable clickableElement = element as IContextClickable;
        if (clickableElement != null)
        {
            if(ignoreReset.value == false)
            {
                clickableElement.RemoveAllListener();
            }

            if (sendGlobal)
                clickableElement.AddListenerOnClick( (ContextElement sender) => { GraphOwner.SendGlobalEvent<ContextElement>(_eventName, sender); } );
            else
            {
                GraphOwner owner = null;

                if(ownerSystem != null)
                    owner = ownerSystem.agent.GetComponent<GraphOwner>();

                if(owner != null)
                    clickableElement.AddListenerOnClick( (ContextElement sender) => { owner.SendEvent<ContextElement>(_eventName, sender); } );
                else
                    clickableElement.AddListenerOnClick( (ContextElement sender) => { SendEvent<ContextElement>(_eventName, sender); } );
            }
            EndAction();
        }
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist or not IContextClickable");
            EndAction(false);
        }

    }
}

}
