using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Test")]
public class TestShareAction : ActionTask
{
    protected override string info
    {
        get
        { 
            return "Test Share Facebook";
        }
    }

    protected override void OnExecute()
    {
        var linkUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "values/misc/FACEBOOK_APP_LINK_URL");
        string title = "Test Title";
        string desc = "Test Description";
        string imageUrl = "https://upload.wikimedia.org/wikipedia/commons/1/1b/Square_200x200.png";

        Debug.LogError(linkUrl.value);
        Debug.LogError(title);
        Debug.LogError(desc);
        Debug.LogError(imageUrl);
#if !UNITY_EDITOR
        SocialManager.Instance.ShareFB(linkUrl.value, title, desc, imageUrl, "",
            (string json) =>
            {
                if(agent != null)
                {
                    if (json.Equals("Error"))
                    {
                        Debug.LogError("FB Share Failed");
                        SendEvent("OnShareError");
                    }
                    else 
                    {
                        Debug.LogError("FB Share Success");
                        SendEvent("OnShareSuccess");
                    }
                }
            });
        EndAction();
#else
        SendEvent("OnShareError");
        EndAction();
#endif        
    }
}

}
