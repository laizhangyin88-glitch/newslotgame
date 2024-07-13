using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Runtime.CompilerServices;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Context")]
    public class SimpleSetContextClickable : ActionTask
    {
        public BBParameter<string> elementName;
        public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
        public BBParameter<string> key;
        public BBParameter<string> eventName;
        public BBParameter<bool> ignoreReset;
        public bool sendGlobal;

        private const string TEXT_ELEMENT_NAME = "Text";

        protected override string info
        {
            get
            {
                if (elementName == null || string.IsNullOrEmpty(elementName.value))
                    return string.Format("({0}) {1}.onClick += {2}{3}", (ignoreReset.value == true) ? "Additive" : "Reset", agentInfo, (sendGlobal ? "(Global)" : ""), eventName);
                else
                    return string.Format("({0}) {1}/{2}.onClick += {3}{4}", (ignoreReset.value == true) ? "Additive" : "Reset", agentInfo, elementName, (sendGlobal ? "(Global)" : ""), eventName);
            }
        }

        protected override void OnExecute()
        {
            ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
            if(elementName.value == "Background")
            {
                Debug.LogError("@@@@@@@@@@@@@@@@@@");
            }
            bool error = true;
            string _eventName = eventName.value;
            if (!string.IsNullOrEmpty(key.value))
            {
                IContextText textElement = element.Find(TEXT_ELEMENT_NAME) as IContextText;
                if (textElement != null)
                {
                    textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, key.value, out error));
                }
            }

            IContextClickable clickableElement = element as IContextClickable;
            if (clickableElement != null)
            {
                if (ignoreReset.value == false)
                {
                    clickableElement.RemoveAllListener();
                }

                if (sendGlobal)
                    clickableElement.AddListenerOnClick((ContextElement sender) => { Debug.LogError("@@@@@@@@@@@@@@@@@..............."); GraphOwner.SendGlobalEvent<ContextElement>(_eventName, sender); });
                else
                {
                    GraphOwner owner = null;

                    if (ownerSystem != null)
                        owner = ownerSystem.agent.GetComponent<GraphOwner>();

                    if (owner != null)
                        clickableElement.AddListenerOnClick((ContextElement sender) => { Debug.LogError("@@@@@@@@@@@@@@@@@..............."); owner.SendEvent<ContextElement>(_eventName, sender); });
                    else
                        clickableElement.AddListenerOnClick((ContextElement sender) => { Debug.LogError("@@@@@@@@@@@@@@@@@..............."); SendEvent<ContextElement>(_eventName, sender); });
                }
                EndAction();
            }
            else
            {
                Debug.LogError("[Context] " + elementName.value + " is not exist or not IContextClickable" + "(" + agent.gameObject.name + "}");
                EndAction(false);
            }

        }
    }

}
