
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_login : ActionTask
{
    public BBParameter<bool> isfirstLogin = false;

    protected override void OnExecute()
    {
        var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
        var dailyBonusResult = BlackboardUtils.FindVariable<Blackboard>(null, "/dailyBonusResult");
        var credit = me.GetValue<long>("credit");
        var pushInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/pushInfo");
        var clientIP = BlackboardUtils.FindVariable<string>(null, "/clientIp");
        var clientCountry = BlackboardUtils.FindVariable<string>(null, "/clientCountry");
        var clientState = BlackboardUtils.FindVariable<string>(null, "/clientState");
        var clientCity = BlackboardUtils.FindVariable<string>(null, "/clientCity");

        string click_pn = "";

        if (pushInfo != null && pushInfo.value != null)
        {
            click_pn = "click_pn";
        }

        if(dailyBonusResult != null && dailyBonusResult.value != null)
            credit -= dailyBonusResult.value.GetValue<long>("earnCredit");

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["distance_from_last_login"] = MainBlackboard.Get().GetValue<long>("distanceFromLastLoginTimestamp");
        customData["click_pn"] = click_pn;

        if(clientIP != null)
            customData["ip_address"] = clientIP.value;
        if(clientCountry != null)
            customData["country"] = clientCountry.value;

        if(clientState != null)
            customData["state"] = clientState.value;
        if(clientCity != null)
            customData["city"] = clientCity.value;
            
        customData["coin"] = MainBlackboard.Get().GetValue<long>("creditBeforeReward");

        customData["os_version"] = ApplicationSettings.GetOperatingSystem();
        customData["locale"] = ApplicationSettings.GetDeviceLanguage();
        customData["device_name"] = ApplicationSettings.GetDeviceModel();
        customData["device_push_setting"] = BlackboardQueryUtils.GetDevicePushSetting();
        Analytics.CustomEvent("client_login", customData);

        if (me.GetValue<int>("loginCount") == 1)
        {
            Analytics.CustomEvent("client_initial_coin", new Dictionary<string, object>
            {
                { "earn_coin", credit }
            });
        }

        if (isfirstLogin.value)
        {
            BICustomEvents.FAS(false);
        }

        AdjustManager.Instance.SendEvent("login");

        EndAction();
    }
}

}
