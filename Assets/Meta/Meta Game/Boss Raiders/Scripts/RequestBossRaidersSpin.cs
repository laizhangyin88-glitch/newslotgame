using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.BossRaiders
{
    [Category("★ BagelCode/Meta Games/Boss Raiders")]
    public class RequestBossRaidersSpin : ActionTask
    {
        public BBParameter<string> contextID;
        public BBParameter<bool> isAutoSpin;
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Boss Raiders Spin"; }
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

            BossRaidersUtils.UpdateBossRaidersData();
            BossRaidersUtils.BackUpInfo();
            BossRaidersUtils.BackUpClubPoint();
            long BetMultiplyNumerator = BossRaidersUtils.CurrentBetMultiplyNumerator;
#if DEV
            BagelCodeClientAPI.RequestBossRaidersDebugSpin(metaGameInfo.id, BetMultiplyNumerator, BossRaidersUtils.DebugSpinType, isAutoSpin.value,
#else
            BagelCodeClientAPI.RequestBossRaidersSpin(metaGameInfo.id, BetMultiplyNumerator, isAutoSpin.value,
#endif
                (response) =>
                {
                    var bb = BossRaidersUtils.BossRaidersInfo;
                    BlackboardUtils.DestroyBlackboard(bb, "wheelResultInfo");

                    ClientAPI2Blackboard.Serialize(bb, response);

                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
                    BossRaidersUtils.UpdateBossRaidersData();
#if DEV
                    BossRaidersUtils.DebugSpinType = BossRaidersDebugSpinType.NONE;
#endif
                    saveAsSuccess.value = true;
                    EndAction();
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
                });
        }
    }
}