using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    [Category("★ BagelCode/Meta Games/Club Arena")]
    public class RequestClubArenaAttack : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<bool> isAutoSpin;
        public BBParameter<bool> saveAsSuccess;

        private string popupContextId;

        protected override string info
        {
            get { return "Request Club Arena Attack (Attack/Steal)"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.CLUB_ARENA);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }

            Blackboard wheelResultBB = ClubArenaUtils.WheelResultInfo;
            if (wheelResultBB == null)
            {
                EndAction(false);
                return;
            }

            bool isSteal = wheelResultBB.GetValue<ClubArenaSpinResultType>("type") == ClubArenaSpinResultType.STEAL;
            long attack = isSteal ? wheelResultBB.GetValue<long>("percentage") : wheelResultBB.GetValue<long>("attack");
            long multiplier = (long)NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.CurrentBetMultiplyNumerator);
            ClubArenaPersonalSimpleWithProfile opponentClass = ClubArenaUtils.OpponentStateClass;
            string userId = opponentClass.userId;
            BagelCodeClientAPI.RequestClubArenaAttack(metaGameInfo.id, isSteal, attack, multiplier, userId, opponentClass.point, contextID.value, ClubArenaUtils.MatchContextID, isAutoSpin.value,
                (response) =>
                {
                    if (agent != null)
                    {
                        var bb = ClubArenaUtils.ClubArenaInfo;
                        ClientAPI2Blackboard.Serialize(bb, response);

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