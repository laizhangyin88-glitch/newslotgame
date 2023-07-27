using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.GemJackpot
{
    [Category("★ BagelCode/Meta Games/Gem Jackpot")]
    public class RequestGemJackpotVideoAdsClaim : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Gem Jackpot Video Ads Claim"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.GEM_JACKPOT);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }

            BagelCodeClientAPI.RequestGemJackpotVideoAdsClaim(metaGameInfo.id,
               (response) =>
               {
                   if (agent != null)
                   {
                       var bb = GemJackpotUtils.GemJackpotInfo;

                       long lastVideoAdsClaimTimestamp = GemJackpotUtils.LastAdsTimestamp;
                       ClientAPI2Blackboard.Serialize(bb, response);

                       GemJackpotUtils.UpdateCloseAdsTimestamp(lastVideoAdsClaimTimestamp);
                       saveAsSuccess.value = true;
                       EndAction();
                   }
               },
               (error) =>
               {
                   switch (error.errorCode)
                   {
                       case ClientModels.Error.INVALID_GEM_JACKPOT_REQUEST_ERROR:
                           EndAction(false);
                           break;
                       default:
                           GlobalErrorHandler.GlobalError(error);
                           break;
                   }
               });
        }
    }
}