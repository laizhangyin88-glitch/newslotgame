using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicPass
{
    [Category("★ BagelCode/Meta Games/Epic Pass")]
    public class RequestSeasonPassInfo : ActionTask
    {
        public BBParameter<List<string>> saveAsImageUrlList;

        protected override string info
        {
            get { return "Request Season Pass Info"; }
        }

        protected override void OnExecute()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }

            BagelCodeClientAPI.SeasonPassInfoRequest(metaGameInfo.id,
                (response) =>
                {
                    // var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "epicPassGameInfo");
                    // ClientAPI2Blackboard.Serialize(bb, response);

                    EpicPassUtils.UpdateSeasonPassInfo(response);

                    saveAsImageUrlList.value = new List<string>();

                    if(!string.IsNullOrEmpty(response.iconBigImageUrl))
                        saveAsImageUrlList.value.Add(response.iconBigImageUrl);
                    if(!string.IsNullOrEmpty(response.iconSmallImageUrl))
                        saveAsImageUrlList.value.Add(response.iconSmallImageUrl);
                    if(!string.IsNullOrEmpty(response.pointIconImageUrl))
                        saveAsImageUrlList.value.Add(response.pointIconImageUrl);
                    if(!string.IsNullOrEmpty(response.titleImageUrl))
                        saveAsImageUrlList.value.Add(response.titleImageUrl);
                    if(!string.IsNullOrEmpty(response.backgroundImageUrl))
                        saveAsImageUrlList.value.Add(response.backgroundImageUrl);
                    if(!string.IsNullOrEmpty(response.decorationLeftImageUrl))
                        saveAsImageUrlList.value.Add(response.decorationLeftImageUrl);
                    if(!string.IsNullOrEmpty(response.decorationRightImageUrl))
                        saveAsImageUrlList.value.Add(response.decorationRightImageUrl);

                    EndAction();
                },
                (error) =>
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            EndAction(false);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }
    }
}