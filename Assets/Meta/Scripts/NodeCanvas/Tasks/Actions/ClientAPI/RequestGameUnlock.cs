using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]

    public class RequestGameUnlock : ActionTask
    {
        public BBParameter<int> gameID;
        public BBParameter<bool> isSuccess;

        public BBParameter<Blackboard> productBB;
        public BBParameter<string> contextId;

        protected override string info
        {
            get
            {
                return "Request Game Unlock(Gem)";
            }
        }

        protected override void OnExecute()
        {
            isSuccess.value = false;

            BagelCodeClientAPI.RequestUnlockGameByGem(gameID.value, contextId.value,
            (response) =>
            {
                if(agent != null)
                {
                    BiEventUtils.ItemAcquired(productBB.value, contextId.value, false);

                    BlackboardQueryUtils.UpdateUnlockGameStatus(response.unlockedGameInfo, response.unlockedSlot);
                    // unlockedSlot

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo(true);

                    isSuccess.value = true;

                    EndAction();
                }

            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.NOT_ENOUGH_GEM_ERROR:
                        {
                            if(agent != null)
                                EndAction();
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });

        }
    }
}
