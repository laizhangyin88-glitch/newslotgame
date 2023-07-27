using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    [Category("★ BagelCode/Meta Games/Club Arena")]
    public class RequestClubArenaEnter : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<bool> isFirstEnter;
        public BBParameter<bool> saveAsSuccess;

        private string popupContextId;

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.CLUB_ARENA);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }
            isFirstEnter.value = ClubArenaUtils.IsFirstEnter();

            string targetUserId = ClubArenaUtils.TargetUserId;
            ClubArenaUtils.TargetUserId = "";
            BagelCodeClientAPI.RequestClubArenaEnter(metaGameInfo.id, isFirstEnter.value, targetUserId, contextID.value,
                (response) =>
                {
                    if (agent != null)
                    {
                        var bb = ClubArenaUtils.ClubArenaInfo;
                        ClientAPI2Blackboard.Serialize(bb, response);

                        ClubArenaUtils.InitEnterResponse();

                        saveAsSuccess.value = true;
                        EndAction();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.NOT_IN_CLUB_ERROR:
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                popupContextId = BiEventUtils.GenerateContextID();
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                info.type = ErrorPopupType.OK;
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);
                                info.callback1 = delegate
                                {
                                    if (agent != null)
                                        ClubArenaUtils.BIClientClickClubArenaPopup("removed_from_club", popupContextId);
                                };

                                ErrorPopupHandler.Instance.OpenError(info);
                                BlackboardQueryUtils.SetMyClubId(0);
                                ClubArenaUtils.BIClientClubArenaPopup("removed_from_club", popupContextId);
                                EndAction(false);
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            EndAction(false);
                            break;
                    }
                });
        }
    }
}