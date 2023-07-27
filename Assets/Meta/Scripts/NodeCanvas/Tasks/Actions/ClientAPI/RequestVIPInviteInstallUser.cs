using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestVIPInviteInstallUser : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<string> userID;
        public BBParameter<int> inviteInstallWithTierId;
        public BBParameter<InviteInstallType> type;

        public BBParameter<bool> isSuccess;

        protected override string info
        {
            get
            {
                return "Request VIP Invite Install User";
            }
        }

        protected override void OnExecute()
        {
            isSuccess.value = false;

            BagelCodeClientAPI.RequestVIPInviteInstallUser(contextID.value, userID.value, inviteInstallWithTierId.value, type.value,
                (response) =>
                {
                    if (agent != null)
                    {
                        isSuccess.value = true;
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardUtils.FindVariable<long>(null, "/me/accRp").value = response.userSyncInfo.accRp;
                        EndAction();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.ALREADY_INVITED_ERROR:
                        case ClientModels.Error.INVALID_INVITER_USER_ID_ERROR:
                            isSuccess.value = false;
                            EndAction();
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }
    }
}