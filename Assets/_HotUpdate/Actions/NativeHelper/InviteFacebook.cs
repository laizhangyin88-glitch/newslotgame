using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class InviteFacebook : ActionTask
{
    public BBParameter<string> requestTitle;
    public BBParameter<string> requestMessage;

    protected override string info
    {
        get
        { 
            return "Invite Facebook";
        }
    }

    protected override void OnExecute()
    {   
#if !UNITY_EDITOR
        SocialManager.Instance.InviteFB(requestMessage.value, requestTitle.value,
            (string json) =>
            {
                // EndAction();
                // if (json.Equals("Error"))
                // {
                //     SendEvent("OnInviteError");
                // }
                // else 
                // {
                //     SendEvent("OnInviteSuccess");
                // }
            });
            EndAction();
#else
        SendEvent("OnInviteError");
        EndAction();
#endif        
    }
}

}
