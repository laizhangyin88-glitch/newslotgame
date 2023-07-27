using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context/Generic")]
public class SimpleSetContextListenable<T> : ActionTask
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<string> eventName;
    public bool sendGlobal;

    protected override string info
    {
        get
        {
            return string.Format("(Reset) {0}.OnValueChanged = {1}{2}", agentInfo, (sendGlobal ? "(Global)" : ""), eventName);
        }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        // bool error = true;
        string _eventName = eventName.value;

        IContextListenable<T> listenableElement = element as IContextListenable<T>;
        if (listenableElement != null)
        {
            if (sendGlobal)
                listenableElement.AddListener( (T value) => { GraphOwner.SendGlobalEvent<T>(_eventName, value); } );
            else
            {
                GraphOwner owner = null;

                if(ownerSystem != null)
                    owner = ownerSystem.agent.GetComponent<GraphOwner>();

                if(owner != null)
                    listenableElement.AddListener( (T value) => { owner.SendEvent<T>(_eventName, value); } );
                else
                    listenableElement.AddListener( (T value) => { SendEvent<T>(_eventName, value); } );
            }
            EndAction();
        }
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist or not IContextListenable");
            EndAction(false);
        }

    }
}

}
