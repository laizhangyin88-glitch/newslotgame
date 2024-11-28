using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/Events")]
    public class SetClickableMeta : ActionTask<ContextElement>
    {
        public BBParameter<string> elementName;
        public BBParameter<string> eventType;
        public BBParameter<string> eventName;
        public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
        public bool sendGlobal;
        public bool ignoreReset;

        protected override string info
            => string.Format("({0}) {1}.onClick += {2}{3}", (ignoreReset ? "Additive" : "Reset"), agentInfo, (sendGlobal ? "(Global)" : ""), eventName);

        protected override void OnExecute()
        {
            ContextElement element = null;
            if(elementName != null && !string.IsNullOrEmpty(elementName.value))
            {
                element = ContextUtils.FindElement(agent, elementName.value, searchingType);
            }

            if (sendGlobal)
            {
                MetaContextElementUtils.SetClickableGlobal(element, eventType.value, eventName.value, !ignoreReset);
            }
            else
            {
                MetaContextElementUtils.SetClickable(element, agent.gameObject, eventType.value, eventName.value, sendGlobal, !ignoreReset);
            }

            EndAction(true);
        }
    }
}
