using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicPass
{
    [Category("★ BagelCode/Meta Games/Epic Pass")]
    public class RequestSeasonPassAdsView : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;
        public BBParameter<long> saveAsAdPoint;

        protected override string info
        {
            get { return "Request Season Pass Ads View"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            saveAsAdPoint.value = EpicPassUtils.AdsFreePoint;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (metaGameInfo == null)
            {
                EndAction();
                return;
            }

            BagelCodeClientAPI.RequestSeasonPassAdsView(metaGameInfo.id,
                (response) =>
                {
                    if(agent != null)
                    {
                        // Update ads timestmap
                        EpicPassUtils.UpdateAdsView(response.lastAdsViewTimestamp, response.nextAdsResetTimestamp);
                        
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