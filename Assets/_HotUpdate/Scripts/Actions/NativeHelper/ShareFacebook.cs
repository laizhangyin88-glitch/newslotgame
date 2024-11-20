using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class ShareFacebook : ActionTask
{
    public BBParameter<string> linkUrl;
    public BBParameter<string> title;
    public BBParameter<string> desc;
    public BBParameter<string> imageUrl;
    public BBParameter<string> encodedAction;

    protected override string info
    {
        get
        { 
            return "Share Facebook";
        }
    }

    protected override void OnExecute()
    {   
#if !UNITY_EDITOR
        SocialManager.Instance.ShareFB(linkUrl.value, title.value, desc.value, imageUrl.value, encodedAction.value,
            (string json) =>
            {
                if (json.Equals("Error"))
                {
                    SendEvent("OnShareError");
                }
                else 
                {
                    SendEvent("OnShareSuccess");
                }
            });
#else
        SendEvent("OnShareError");
#endif  
        EndAction();
    }
}

}
