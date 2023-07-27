using UnityEngine;
using BagelCode.ClientModels;
using ParadoxNotion;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class LobbyVegasDreamsIcon : LobbyMetaIcon
    {
        private Coroutine requestVegasDreamInfoCoroutine = null;
        public override void OnStart()
        {
            if (requestVegasDreamInfoCoroutine != null)
                scrollContextElement?.StopCoroutine(requestVegasDreamInfoCoroutine);
            requestVegasDreamInfoCoroutine = scrollContextElement?.StartCoroutine(UpdateIconData());
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {
            if (iconObject != null)
            {
                int eventId = (int)eventData.value;
                var eventInfo = PassiveEventManager.Instance.GetEventInfoFromID(eventId, true);
                bool isDestroy = false;
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_REFRESH_PASSIVE) &&
                    eventInfo != null &&
                    (eventInfo.type == EventInfoType.BUILD_DREAM_SEASON ||
                    eventInfo.type == EventInfoType.VIP_LOUNGE))
                {
                    isDestroy = true;
                }
                else if (eventName.Equals(MetaEventDefine.ON_META_UI_EVENT) && eventData.name.Equals(ON_REFRESH_META_GAME) &&
                    eventInfo != null &&
                    (eventInfo.type == EventInfoType.BUILD_DREAM_SEASON ||
                    eventInfo.type == EventInfoType.VIP_LOUNGE))
                {
                    isDestroy = true;
                }

                if (isDestroy)
                {
                    bool isActive =
                        BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.BUILD_DREAM_SEASON) &&
                        BlackboardQueryUtils.IsActiveMetaGameEvent(EventInfoType.VIP_LOUNGE);

                    if (!isActive)
                    {
                        //BlackboardQueryUtils.SetVegasDreamsActive(false);
                        DestroyIconObject();
                        OnStart();
                    }
                }
            }
            else
            {
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_START_PASSIVE))
                {
                    if (ChackEventInfoType(((EventData<EventInfoType>)eventData).value))
                        OnStart();
                }
            }
        }

        protected override bool CheckActiveEvent()
        {
            return BlackboardQueryUtils.IsVegasDreamsActive();
        }

        protected override void OnInitCheckEventInfoTypeList()
        {
            base.OnInitCheckEventInfoTypeList();

            checkEventInfoTypeList.Add(EventInfoType.BUILD_DREAM_SEASON);
            checkEventInfoTypeList.Add(EventInfoType.VIP_LOUNGE);
        }

        private IEnumerator UpdateIconData()
        {
            yield return new WaitUntil(() => !LobbyMetaIconController.IsCalledApiVipLoungeInfo);
            //bool isSuccess = false, isFail = false;
            //BagelCodeClientAPI.RequestVegasDreamInfo(
            //    (response) =>
            //    {
            //        BlackboardQueryUtils.UpdateRequestVegasDreamInfo(response);
            //        isSuccess = true;
            //    },
            //    (error) =>
            //    {
            //        isFail = true;
            //    });

            //yield return new WaitUntil(() => isSuccess || isFail);
            UpdateVegasDreamsIconData();
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(ON_REFRESH_ICON_CHECK));
        }

        private void UpdateVegasDreamsIconData()
        {
            bool isActive = CheckActiveEvent();
            if (isActive)
            {
                bundleName = VegasDreams.VegasDreams.Defines.COMMON_BUNDLE;
                sharedBundleName = "";
                iconAssetName = LOBBY_BUTTON_ASSET_NAME;

                eventType = EventInfoType.UNKNOWN;
                eventId = 0;
                endTimestamp = 0;

                enableShare = false;
                eventName = "";

                bundleList = new List<string>() { VegasDreams.VegasDreams.Defines.COMMON_BUNDLE };
            }
            else
            {
                bundleName = "";
                sharedBundleName = "";
                iconAssetName = "";

                eventType = EventInfoType.UNKNOWN;
                eventId = 0;
                endTimestamp = 0;

                enableShare = false;
                eventName = "";

                bundleList = null;
            }

            if (isActive)
            {
                if (iconObject == null)
                    MakeEventButton("Vegas Dreams Event Button");
            }
            else
                DestroyIconObject();
        }
    }
}
