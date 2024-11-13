using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_deeplink : ActionTask
{
    public BBParameter<string> url;
    public BBParameter<string> couponId;

    protected override void OnExecute()
    {
        string coupon = null;
        if (couponId != null && !string.IsNullOrEmpty(couponId.value))
        {
            coupon = couponId.value;
        }

        Analytics.CustomEvent("client_deeplink", new Dictionary<string, object>
        {
            { "deeplink_url", url.value },
            { "coupon_id", coupon }
        });

        EndAction();
    }
}

}
