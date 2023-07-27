using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Popup/Vip Rewards")]
public class GetVipDealButtonBB : ActionTask<Blackboard> 
{
    public BBParameter<long> saveAsEndTimestamp;
    public BBParameter<long> saveAsWarningTimestamp;
    public BBParameter<bool> saveAsIsLock;
    public BBParameter<bool> saveAsisAvailable;
    public BBParameter<int>  saveAsVipDealInfoID;

    private long ONE_HOUR = 3600000L;

    protected override void OnExecute()
    {
        saveAsEndTimestamp.value = 0L;
        saveAsWarningTimestamp.value = 0L;
        saveAsVipDealInfoID.value = 0;
        saveAsIsLock.value = true;
        saveAsisAvailable.value = false;

        var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();

        if(vipDealInfo != null)
        {
            var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");
            saveAsVipDealInfoID.value = vipDealInfo.GetValue<int>("vipDealInfoId");

            saveAsEndTimestamp.value = endTimestamp;
            saveAsWarningTimestamp.value = endTimestamp - ONE_HOUR;
            saveAsIsLock.value = BlackboardQueryUtils.IsVipDealTierLock();

            saveAsisAvailable.value = true;
        }

        EndAction();
    }
}

}
