using BagelCode.ClientModels;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using UnityEngine;

namespace BagelCode
{
    public class ClubNewsFeedRequestFinderCellController : ClubNewsFeedRequestCellController
    {
        private bool isSending = false;

        protected override string GetReceivedText()
        {
            return StringTableUtils.GetString(GLOBAL, "HIDDEN_OBJECTS_NEWS_FEED_RECEIVED");
        }

        protected override string GetTitleText()
        {
            return StringTableUtils.GetString(GLOBAL, "HIDDEN_OBJECTS_NEWS_FEED_TITLE");
        }

        protected override void OnClickGift()
        {
            StartCoroutine(SendGiftCoroutine());
        }

        protected override void InitEvents()
        {
            base.InitEvents();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.HiddenObjects.Events.ON_UPDATE_FINDER_COUNT, OnUpdateFinder);
        }

        private void OnUpdateFinder(EventData eventData)
        {
            UpdateCellData();
        }

        private IEnumerator SendGiftCoroutine()
        {
            if (isSending) yield break;
            isSending = true;

            // Loading
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            // Gift Request
            long clubId = BlackboardQueryUtils.GetMyClubId();
            long feedId = feedInfo.GetValue<long>("id");
            bool success = false;
            bool fail = false;
            BagelCodeClientAPI.RequestHiddenObjectsFinderGift(clubId, feedId,
                (response) =>
                {
                    success = true;

                    BlackboardUtils.SetOrCreateValue(feedInfo, "like", response.clubFeed.like);

                    var finderRequest = response.clubFeed.content as ClubFeedHiddenUniverseFinderRequest;
                    BlackboardUtils.SetOrCreateValue(feedInfo, "requirement", finderRequest.requirement);

                    HiddenObjects.HiddenObjects.Utils.UpdateFinderCount(response.finder);
                },
                (error) =>
                {
                    fail = true;
                    switch (error.errorCode)
                    {
                        case Error.SOMEONE_UPDATING_CLUB_ERROR:
                            GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                            break;
                        case Error.NOT_IN_CLUB_ERROR:
                            {
                                var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                                if (meClubID.value > 0)
                                {
                                    bool stringError = false;
                                    ErrorPopupInfo info = new ErrorPopupInfo();
                                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                    info.type = ErrorPopupType.OK;
                                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

                                    ErrorPopupHandler.Instance.OpenError(info);
                                }

                                BlackboardQueryUtils.SetMyClubId(0);
                                MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });

            yield return new WaitUntil(() => success || fail);

            MetaPopupUtils.ClosePopup(loadingObj);

            isSending = false;
        }
    }
}
