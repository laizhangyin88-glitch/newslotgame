using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestClubJoinAcceptAll : ActionTask<Blackboard> 
{
    public BBParameter<long> clubID;

    public BBParameter<bool> isSuccess;

    protected override string info 
    {
        get { return "Club Join Requests Accept All"; }
    }

    protected override void OnExecute()
    {
        isSuccess.value = false;

        BagelCodeClientAPI.ClubJoinRequestAcceptAll(clubID.value, 
        (response) =>
        {
            if(agent != null)
            {
                if (response.success)
                {
                    isSuccess.value = true;
                }
                else
                {
                    if (response.vacancies > 0)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_ALL_MAX_MEMBER", response.vacancies);
                        isSuccess.value = false;
                    }
                    else
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_MAX_MEMBER");
                        isSuccess.value = false;
                    }
                }
                
                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.INVALID_CLUB_REQUEST_ERROR:
                case ClientModels.Error.CLUB_ALREADY_DECLINED_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_NO_LONGER");
                        isSuccess.value = true;
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.ALREADY_IN_CLUB_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_ALREADY");
                        isSuccess.value = true;
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.CLUB_REACHED_MAX_MEMBER_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_ACCEPT_MAX_MEMBER");
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.WRONG_MEMBER_PERMISSION_ERROR:
                case ClientModels.Error.CLUB_PERMISSION_ERROR:
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

