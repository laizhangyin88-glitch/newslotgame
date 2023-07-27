using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestClubJoinDeclineAll : ActionTask<Blackboard> 
{
    public BBParameter<long> clubID;
    
    public BBParameter<bool> isSuccess;

    protected override string info 
    {
        get { return "Club Join Requests Decline All"; }
    }

    protected override void OnExecute()
    {
        isSuccess.value = false;

        BagelCodeClientAPI.ClubJoinRequestDeclineAll(clubID.value, 
        (response) =>
        {
            if(agent != null)
            {
                isSuccess.value = true;

                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.INVALID_CLUB_REQUEST_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_NO_LONGER");
                        isSuccess.value = true;
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.WRONG_MEMBER_PERMISSION_ERROR:
                case ClientModels.Error.CLUB_PERMISSION_ERROR:
                    // Close Popup & Refresh Club Info
                    if(agent != null)
                    {
                        isSuccess.value = true;
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}

