using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetPurchaseCoinBoosterBB : ActionTask<Blackboard> 
{
    public BBParameter<string>  valueA;
    public BBParameter<long> prevEarn;
    public BBParameter<EventInfoType> evnetInfoType;
    public BBParameter<long>   rewardPoint;
    public BBParameter<float>  origItemPrice;
    public BBParameter<float>  itemPrice;
    public BBParameter<List<double>> wheelViewMultiplierList;
    public BBParameter<List<double>> eventWheelViewMultiplierList;
    public BBParameter<List<long>> wheelMultiplierNumeratorList;
    public BBParameter<List<long>> eventWheelMultiplierNumeratorList;

    public BBParameter<double> initMultiplier;
    public BBParameter<long> initGoods;
    public BBParameter<Blackboard> boosterItemBB;

    public BBParameter<ItemType> boosterItemType;           // ItemType.CREDIT_MULTIPLIER_WHEEL
    public BBParameter<EventInfoType> allEventInfoType;     // EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY
    public BBParameter<EventInfoType> minEventInfoType;     // EventInfoType.CREDIT_MULTIPLIER_WHEEL

    protected override string info
    {
        get { return "Get Purchase Booster Item BB"; }
    }

    protected override void OnExecute()
    {
        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        boosterItemBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, boosterItemType.value);

        itemPrice.value        = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "price").value);
        origItemPrice.value    = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);

        var   rp         = BlackboardUtils.FindVariable<long>(boosterItemBB.value, "rp");
        var   settingList = BlackboardUtils.FindVariable<List<Blackboard>>(boosterItemBB.value, "setting");

        if(evnetInfoType.value == allEventInfoType.value)
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(allEventInfoType.value);

            if(eventInfo != null)
            {
                long multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                UpdateAllMultiplierEvent(settingList.value, multiplierNumerator);
            }
            else
            {
                UpdateMinMultiplierEvent(settingList.value, NumberUtils.GetGlobalDenominator());
            }
        }
        else
        {
            long minMultiplierNumerator = NumberUtils.GetGlobalDenominator();
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(minEventInfoType.value);
            if(eventInfo != null)
            {
                minMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            }

            UpdateMinMultiplierEvent(settingList.value, minMultiplierNumerator);
        }

        rewardPoint.value = rp.value;

        EndAction();
    }

    private void UpdateAllMultiplierEvent(List<Blackboard> settingList, long eventMultiplierNumerator)
    {
        wheelViewMultiplierList.value = new List<double>();
        eventWheelViewMultiplierList.value = new List<double>();
        wheelMultiplierNumeratorList.value = new List<long>();
        eventWheelMultiplierNumeratorList.value = new List<long>();

        double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

        for(int i=0; i<settingList.Count; ++i)
        {
            long multiplierNumerator = settingList[i].GetValue<long>("multiplierNumerator");
            wheelMultiplierNumeratorList.value.Add(multiplierNumerator);
            eventWheelMultiplierNumeratorList.value.Add( NumberUtils.GetMultiplierNumeratorValue(multiplierNumerator, eventMultiplierNumerator) );

            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
            wheelViewMultiplierList.value.Add(multiplier);
            eventWheelViewMultiplierList.value.Add(multiplier * eventMultiplier);

            if(i == 0)
            {
                initMultiplier.value = eventWheelViewMultiplierList.value[0];
                initGoods.value = NumberUtils.GetMultiplierNumeratorValue(prevEarn.value, eventWheelMultiplierNumeratorList.value[0]);
            }
        }
    }

    private void UpdateMinMultiplierEvent(List<Blackboard> settingList, long eventMultiplierNumerator)
    {
        wheelViewMultiplierList.value = new List<double>();
        eventWheelViewMultiplierList.value = new List<double>();
        wheelMultiplierNumeratorList.value = new List<long>();
        eventWheelMultiplierNumeratorList.value = new List<long>();

        double minMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

        for(int i=0; i<settingList.Count; ++i)
        {
            long multiplierNumerator = settingList[i].GetValue<long>("multiplierNumerator");
            wheelMultiplierNumeratorList.value.Add(multiplierNumerator);

            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
            wheelViewMultiplierList.value.Add(multiplier);

            if(eventMultiplierNumerator > multiplierNumerator)
            {
                eventWheelViewMultiplierList.value.Add(minMultiplier);
                eventWheelMultiplierNumeratorList.value.Add(eventMultiplierNumerator);
            }
            else
            {
                eventWheelViewMultiplierList.value.Add(multiplier);
                eventWheelMultiplierNumeratorList.value.Add(multiplierNumerator);
            }

            if(i == 0)
            {
                initMultiplier.value = eventWheelViewMultiplierList.value[0];
                initGoods.value = NumberUtils.GetMultiplierNumeratorValue(prevEarn.value, eventWheelMultiplierNumeratorList.value[0]);
            }
        }
    }
}

}
