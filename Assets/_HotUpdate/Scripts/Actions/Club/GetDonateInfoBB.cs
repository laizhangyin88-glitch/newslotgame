using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetDonateInfoBB : ActionTask<Blackboard>
{
    public BBParameter<Blackboard> clubMemberInfoBB;
    public BBParameter<Blackboard> clubInfo;
    public BBParameter<float> currentDonateValue;
    public BBParameter<int> dailyDonationCount;
    public BBParameter<long> meCoins;
    public BBParameter<ContextElement> donateButtonElement;

    public BBParameter<string> saveDonateCoinText;
    public BBParameter<long> saveDonateCoin;
    public BBParameter<int> saveDonateLevel;

    private float prevDonateValue = -1f;

    protected override string info
    {
        get { return "Get Donate Info BB"; }
    }

    protected override void OnExecute()
    {
        if(clubMemberInfoBB != null && clubMemberInfoBB.value != null && prevDonateValue != currentDonateValue.value)
        {
            prevDonateValue = currentDonateValue.value;

            var clubLevel = clubInfo.value.GetValue<int>("level");

            saveDonateLevel.value = (int)currentDonateValue.value - dailyDonationCount.value;
            saveDonateCoin.value = ClubUtils.GetDonateUnitCost(clubLevel) * saveDonateLevel.value;

            bool isError = false;
            saveDonateCoinText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_DONATE_COIN_TEXT", saveDonateCoin.value, out isError);

            if(donateButtonElement != null && donateButtonElement.value != null)
            {
                IContextBooleanProperty button = donateButtonElement.value as IContextBooleanProperty;
                button.SetBooleanProperty(saveDonateCoin.value <= meCoins.value);
            }
            

        }

        EndAction();
    }
}

}
