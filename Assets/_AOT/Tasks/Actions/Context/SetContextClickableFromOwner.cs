using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextClickableFromOwner : ActionTask<ContextElement>
{
    public BBParameter<Transform> ownerTransform;
    public BBParameter<string> eventName;
    public BBParameter<bool>   ignoreReset;
    public bool sendGlobal;

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
        IContextClickable clickableElement = agent as IContextClickable;
        if (clickableElement != null)
        {
            if(ignoreReset.value == false)
            {
                clickableElement.RemoveAllListener();
            }
            
            if (sendGlobal)
                clickableElement.AddListenerOnClick( (ContextElement sender) => { GraphOwner.SendGlobalEvent<ContextElement>(eventName.value, sender); } );
            else 
            {
                GraphOwner owner = null;

                if(ownerTransform != null && ownerTransform.value != null)
                    owner = ownerTransform.value.GetComponent<GraphOwner>();

                if(owner != null)
                    clickableElement.AddListenerOnClick( (ContextElement sender) => { owner.SendEvent<ContextElement>(eventName.value, sender); } );
                else
                    clickableElement.AddListenerOnClick( (ContextElement sender) => { SendEvent<ContextElement>(eventName.value, sender); } );
            }
        }
        EndAction();
    }
}

}
