using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicPass
{
    [Category("★ BagelCode/Meta Games/Epic Pass")]
    public class RequestSeasonPassAdsClaim : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Season Pass Ads Claim"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (metaGameInfo == null)
            {
                EndAction();
                return;
            }

            BagelCodeClientAPI.RequestSeasonPassAdsClaim(metaGameInfo.id,
                (response) =>
                {
                    if(agent != null)
                    {
                        EpicPassUtils.UpdateAdsClaim(response.updateInfo, response.adsFreePoint, response.lastAdsClaimTimestamp);
                        saveAsSuccess.value = true;
                        EndAction();
                    }
                    
                },
                (error) =>
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            EndAction();
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