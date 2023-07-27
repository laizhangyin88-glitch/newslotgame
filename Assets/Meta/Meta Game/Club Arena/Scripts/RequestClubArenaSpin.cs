using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    [Category("★ BagelCode/Meta Games/Club Arena")]
    public class RequestClubArenaSpin : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<bool> isAutoSpin;
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

            ClubArenaUtils.UpdateClubArenaData();
            ClubArenaUtils.BackUpInfo();
            long BetMultiplyNumerator = ClubArenaUtils.CurrentBetMultiplyNumerator;
#if DEV
            BagelCodeClientAPI.RequestClubArenaDebugSpin(metaGameInfo.id, BetMultiplyNumerator, ClubArenaUtils.DebugSpinType, isAutoSpin.value, ClubArenaUtils.MyStateClass.shield, ClubArenaUtils.MatchContextID,
#else
            BagelCodeClientAPI.RequestClubArenaSpin(metaGameInfo.id, BetMultiplyNumerator, isAutoSpin.value, ClubArenaUtils.MyStateClass.shield, ClubArenaUtils.MatchContextID,
#endif
                (response) =>
                {
                    if (agent != null)
                    {
                        var bb = ClubArenaUtils.ClubArenaInfo;
                        BlackboardUtils.DestroyBlackboard(bb, "result");

                        ClientAPI2Blackboard.Serialize(bb, response);

                        if (response.userSyncInfo != null)
                        {
                            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                            BlackboardQueryUtils.ApplyUserSyncInfo();
                        }

                        ClubArenaUtils.UpdateClubArenaData();
                        ClubArenaUtils.CheckEnergyForSpinResult();
#if DEV
                        ClubArenaUtils.DebugSpinType = ClubArenaDebugSpinResultType.NONE;
#endif
                        saveAsSuccess.value = true;
                        EndAction();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.NOT_ENOUGH_ENERGY_ERROR:
                            EndAction(false);
                            break;
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
                                    {
                                        Blackboard bb = agent.GetComponent<Blackboard>();
                                        if (bb != null)
                                            BlackboardUtils.SetOrCreateValue(bb, "isRemovedFromClub", true);
                                        ClubArenaUtils.BIClientClickClubArenaPopup("remove_from_club", popupContextId);
                                    }
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