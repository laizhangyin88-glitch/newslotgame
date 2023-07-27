using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.BossRaiders
{
    [Category("★ BagelCode/Meta Games/Boss Raiders")]
    public class RequestBossRaidersEnter : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Boss Raiders Enter"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.BOSS_RAIDERS);
            if (metaGameInfo == null)
            {
                EndAction(false);
                return;
            }

            BagelCodeClientAPI.RequestBossRaidersEnter(metaGameInfo.id,
                (response) =>
                {
                    if(agent != null)
                    {
                        var bb = BossRaidersUtils.BossRaidersInfo;
                        ClientAPI2Blackboard.Serialize(bb, response);

                        if(response.clubInfo != null)
                        {
                            var clubInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "clubInfo");
                            ClientAPI2Blackboard.Serialize(clubInfoBB, response.clubInfo);
                        }

                        BossRaidersUtils.InitEnterResponse();

                        saveAsSuccess.value = true;
                        EndAction();
                    }
                },
                (error) =>
                {
                    if(agent != null)
                    {
                        switch(error.errorCode)
                        {
                            case ClientModels.Error.NOT_IN_CLUB_ERROR:
                                {
                                    bool stringError = false;
                                    ErrorPopupInfo info = new ErrorPopupInfo();
                                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                    info.type = ErrorPopupType.OK;
                                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);
                                    info.callback1 = delegate
                                    {
                                        BossRaidersUtils.BIClientClickBossRaidersPopup(contextID.value, "remove");
                                    };

                                    ErrorPopupHandler.Instance.OpenError(info);
                                    BlackboardQueryUtils.SetMyClubId(0);
                                    BossRaidersUtils.BIClientBossRaidersPopup(contextID.value, "remove");
                                    EndAction(false);
                                }
                                break;
                            default:
                                GlobalErrorHandler.GlobalError(error);
                                EndAction(false);
                                break;
                        }
                    }

                }
            );
        }
    }
}
