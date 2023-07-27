using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode;
using ParadoxNotion;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Context")]
    public class SetContextClickableFromOwnerMeta : ActionTask<ContextElement>
    {
        public BBParameter<Transform> ownerTransform;
        public BBParameter<string> eventName;
        public BBParameter<bool> ignoreReset;
        public bool sendGlobal;

        protected override string info
        {
            get
            {
                if (ignoreReset.value == true)
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
                if (ignoreReset.value == false)
                {
                    clickableElement.RemoveAllListener();
                }

                if (sendGlobal)
                {
                    clickableElement.AddListenerOnClick((ContextElement sender) =>
                    {
                        var eventData = new EventData<ContextElement>(eventName.value, sender);
                        EventSender.SendGlobalEvent(eventData);
                    });
                }
                else
                {
                    GameObject owner = agent.gameObject;
                    if (ownerTransform != null && ownerTransform.value != null)
                    {
                        owner = ownerTransform.value.gameObject;
                    }

                    clickableElement.AddListenerOnClick((ContextElement sender) =>
                    {
                        if(owner != null)
                        {
                            var eventData = new EventData<ContextElement>(eventName.value, sender);
                            EventSender.SendEvent(owner, eventData);
                        }
                    });
                }
            }

            EndAction();
        }
    }
}
