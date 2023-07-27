using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public class ProfileEventGroupEventMultiplier : ProfileEventGroup
    {
        protected override float DisplayTime => 4f;

        private bool isReset = true;

        private ContextElement boostEventElement;

        private List<EventInfo> expBoostEventInfoList;
        private List<EventInfo> expBoostExtendableEventInfoList;

        private double eventMulti = 0.0;
        private long endTimestamp = 0L;

        private EventTagController tagController;

        public ProfileEventGroupEventMultiplier(ContextElement _root) : base(_root)
        {

        }

        private void UpdateEventLists()
        {
            expBoostEventInfoList = PassiveEventManager.Instance.GetActiveEventInfoList(EventInfoType.EXP_MULTIPLY);
            expBoostExtendableEventInfoList = PassiveEventManager.Instance.GetActiveEventInfoList(EventInfoType.EXP_MULTIPLY_EXTENDABLE);
        }

        public override bool IsAvailable()
        {
            UpdateEventLists();

            return (expBoostEventInfoList != null && expBoostEventInfoList.Count > 0) ||
                (expBoostExtendableEventInfoList != null && expBoostExtendableEventInfoList.Count > 0);
        }

        // todo refactoring
        // 갱신 상황 명확히
        public override void UpdateTagState()
        {
            if (!isInit) return;

            UpdateEventLists();

            UpdateEventDatas();

            InitEventTagController();
        }

        protected override void OnAppearTag()
        {
            if (isReset)
            {
                isReset = false;
                tagController.ResetIndex();
            }
        }

        protected override void OnDisappearTag()
        {
            isReset = true;
        }

        private void UpdateEventDatas()
        {
            long shortestEventEndTimestamp = long.MaxValue;

            // total exp boost
            long totalMultiplierNumerator = 100L;
            if (expBoostEventInfoList != null && expBoostEventInfoList.Count > 0)
            {
                foreach (var eventInfo in expBoostEventInfoList)
                {
                    totalMultiplierNumerator += PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo) - 100L;

                    if (eventInfo.endTimestamp < shortestEventEndTimestamp)
                        shortestEventEndTimestamp = eventInfo.endTimestamp;
                }
            }

            // most one multiplier exp boost extendable
            if (expBoostExtendableEventInfoList != null && expBoostExtendableEventInfoList.Count > 0)
            {
                long maxMultiplierNumerator = 0L;

                foreach (var eventInfo in expBoostExtendableEventInfoList)
                {
                    long multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                    if (multiplierNumerator > maxMultiplierNumerator)
                        maxMultiplierNumerator = multiplierNumerator;

                    if (eventInfo.endTimestamp < shortestEventEndTimestamp)
                        shortestEventEndTimestamp = eventInfo.endTimestamp;
                }
                totalMultiplierNumerator += maxMultiplierNumerator - 100L;
            }

            eventMulti = NumberUtils.GetMultiplierFromNumerator(totalMultiplierNumerator);
            endTimestamp = shortestEventEndTimestamp;
        }

        protected override void Initialize()
        {
            if (isInit) return;
            isInit = true;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Event Tag Custom";
            Transform parent = eventTagAreaElement.transform;

            tagObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            anim = tagObj.GetComponent<Animator>();
            anim.SetBool("Appear", false);

            eventTagAreaElement.UpdateContext(true);
            boostEventElement = ContextUtils.FindElement(eventTagAreaElement, "Event Tag Custom", ContextSearchingType.ChildrenSearch);

            UpdateTagState();
        }

        private void InitEventTagController()
        {
            string eventMultiplierText = StringTableUtils.GetString(
                StringTable.StringTableType.Global,
                "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER",
                eventMulti);

            tagController = boostEventElement.GetComponent<EventTagController>();
            tagController.Initialize(
                endTimestamp,
                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                "",
                "Ended",
                true,
                tagObj,
                new List<string>() { eventMultiplierText },
                2f,
                0.5f);
        }
    }
}
