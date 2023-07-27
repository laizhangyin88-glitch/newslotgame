using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestInviterUser : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<string> userID;
        public BBParameter<InviteInstallType> type;

        public BBParameter<bool> isSuccess;

        protected override string info
        {
            get
            {
                return "Request Inviter User";
            }
        }

        protected override void OnExecute()
        {
            isSuccess.value = false;

            BagelCodeClientAPI.RequestInviterUser(contextID.value, userID.value, type.value,
                (response) =>
                {
                    if (agent != null)
                    {
                        isSuccess.value = true;
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