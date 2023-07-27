using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetDonateInitValue : ActionTask<Blackboard>
{
    public BBParameter<Blackboard> clubInfo;

    public BBParameter<int> dailyDonationCount;
    public BBParameter<int> saveDonateValue;

    protected override string info
    {
        get { return string.Format("{0} = Get Donate Init Value", saveDonateValue); }
    }

    protected override void OnExecute()
    {
        if(clubInfo != null && clubInfo.value != null)
        {
            var clubLevel = clubInfo.value.GetValue<int>("level");

            int maxDonateCount = ClubUtils.GetMaxDonateCount();

            long meCoins = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/credit").value;

            saveDonateValue.value = maxDonateCount;

            for(int i=maxDonateCount; i > dailyDonationCount.value; --i)
            {
                long donateCost = ClubUtils.GetDonateUnitCost(clubLevel) * (long)(i - dailyDonationCount.value);
                
                if(meCoins >= donateCost)
                {
                    saveDonateValue.value = i;
                    break;
                }
            }
        }

        EndAction();
    }
}

}
