using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Lobby")]
    public class MakeLobbySideEventButton : ActionTask<Blackboard>
    {
        public BBParameter<string> assetName;
        public BBParameter<bool> isVipDeal;
        public BBParameter<bool> isMetaEvent;
        public BBParameter<bool> isOtherEvent;
        public BBParameter<bool> isHiddenObjects;
        public BBParameter<GameObject> emptyObj;
        public BBParameter<GameObject> saveAsButtonObj;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private ContextElement scrollContentsElement;
        private bool isInit = false;

        protected override string info
        {
            get
            {
                return string.Format("Make Side Event Button [{0}]", assetName.value);
            }
        }

        protected override void OnExecute()
        {
            if (saveAsButtonObj.value != null)
                Object.Destroy(saveAsButtonObj.value);

            if (CheckActiveEvent())
            {
                Init();

                ContextElement metaEventButtonElement = MetaObjectUtils.MakePrefab(
                    MetaStringDefine.LOBBY_BUNDLE_NAME, assetName.value, scrollContentsElement.transform,
                    "", assetName.value).GetComponent<ContextElement>();

                saveAsButtonObj.value = metaEventButtonElement.gameObject;
            }

            EndAction();
        }

        private void Init()
        {
            if (isInit) return;

            ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
            agentElement.UpdateContext(false);

            scrollContentsElement = ContextUtils.FindElement(agentElement, "Event Button Area", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        private bool CheckActiveEvent()
        {
            bool isActive = false;

            if (isVipDeal.value)
                isActive = !BlackboardQueryUtils.IsVipDealTierLock() && BlackboardQueryUtils.GetActiveVipDealInfo() != null;
            else if (isMetaEvent.value)
                isActive = BlackboardQueryUtils.GetMetaGameEventInfo() != null;
            else if (isOtherEvent.value)
                isActive = BlackboardQueryUtils.GetOtherMetaGameEventInfo() != null;
            else if (isHiddenObjects.value)
                isActive = true;

            return isActive;
        }
    }
}
