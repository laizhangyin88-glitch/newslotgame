using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_videoad : ActionTask
{
    public BBParameter<string> action;
    public BBParameter<string> rewardType;
    public BBParameter<long>   rewardAmount;
    public BBParameter<string> placement;
    public BBParameter<string> typeOfAd;

    private const string VUNGLE = "vungle";
    private const string IRONSOURCE = "ironsource";

    protected override void OnExecute()
    {
        if(typeOfAd.value != "inhouse")
        {
#if UNITY_WSA
            typeOfAd.value = VUNGLE;
#else
            typeOfAd.value = IRONSOURCE;
#endif
        }

        Analytics.CustomEvent("client_video_ad", new Dictionary<string, object>
        {
            { "action", action.value },
            { "type_of_reward", rewardType.value },
            { "amount_of_reward", rewardAmount.value },
            { "placement", placement.value },
            { "type_of_ad", typeOfAd.value }
        });

        EndAction();
    }
}

}
