using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]

    public class RequestClubMissionInfo : ActionTask<Blackboard>
    {
        public BBParameter<string> missionDateValue;
        public BBParameter<string> missionIDValue;

        protected override string info
        {
            get
            {
                return "Request Club Mission Info";
            }
        }

        protected override void OnExecute()
        {
            var missionDate = BlackboardUtils.FindVariable<int>(agent, missionDateValue.value);
            var missionID = BlackboardUtils.FindVariable<string>(agent, missionIDValue.value);

            BagelCodeClientAPI.RequestClubMissionInfo(missionDate.value, missionID.value,
            (response) =>
            {
                if (agent != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "clubMissionResponse");

                    BlackboardUtils.ClearBlackboard(bb);
                    ClientAPI2Blackboard.Serialize(bb, response);
                    EndAction(true);
                }
            },
            (error) =>
            {
                switch (error.errorCode)
                {
                    case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        if (agent != null)
                            EndAction(true);
                        break;
                    case ClientModels.Error.NOT_IN_CLUB_ERROR:
                        {
                            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                            if (meClubID.value > 0)
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                info.type = ErrorPopupType.OK;
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                                ErrorPopupHandler.Instance.OpenError(info);
                            }

                            BlackboardQueryUtils.SetMyClubId(0);
                            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
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
