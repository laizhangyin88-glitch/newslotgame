using BagelCode.ClientModels;
using BagelCode.VipLounge;
using BagelCode;
using SlotMaker.Slots.Tasks.Actions.Game;
using SlotMaker.Tasks.Actions;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;

public class InGameLineController : MonoBehaviour
{
    List<long>  betListBase;
    Variable<long> baseWager;
    Variable<int> line;


    ContextButton cxeRootBtn;
    //ContextTextMeshPro cxeText;
    IContextText cxeText;

    protected  void Awake()
    {
    }
    void Start()
    {
        betListBase = new List<long>(BlackboardUtils.FindVariable<List<long>>("./betList").value);
        baseWager = BlackboardUtils.FindVariable<long>("./game/baseWager");
        //baseWager = BlackboardUtils.FindValue<long>(null, "./game/baseWager");
        line = BlackboardUtils.FindVariable<int>("./gameNew/selectLine");


        cxeRootBtn = transform.GetComponent<ContextButton>();
        cxeRootBtn.UpdateContext(true);
        cxeText = ContextUtils.FindElement(cxeRootBtn, "Text Line", ContextSearchingType.ChildrenSearch) as IContextText;

        Debug.Log(cxeText);

    }

    [Button]
    void test_get()
    {
        long temp = BlackboardUtils.FindVariable<long>("./game/baseWager").value;
        
        Debug.LogError($"baseWager = {temp}");
    }

    public void OnLineClick()
    {
        line.value++;
        if (line.value > baseWager.value)
            line.value = 1;

        List<long> betListNow = new List<long>();
        foreach (long item in betListBase)
        {
            betListNow.Add(line.value * item);
        }

        int betIndex = BlackboardUtils.FindVariable<int>("./betIndex").value;
        var cb = ContentBlackboard.Get();
        BlackboardUtils.SetOrCreateValue<long>(cb, "maxBetCredit", betListNow[betListNow.Count - 1]);
        BlackboardUtils.SetOrCreateValue(cb, "betList", betListNow);

        /*
        //BlackboardUtils.SetOrCreateValue<int>(null, "./gameNew/selectLine", line.value); 
        BlackboardUtils.SetOrCreateValue<long>(cb, "betCredit", betListNow[betIndex]);        
        BlackboardUtils.SetOrCreateValue<long>(cb, "totalBetCredit", betListNow[betIndex]);
        */

        cxeText.SetText(line.value.ToString());
        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdateBetCredit", betListNow[betIndex]));
        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdatedTotalBetCredit", betListNow[betIndex]));
        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<int>("Line", line.value));

        //【 MessageDispatcher 发送消息】：eventName = OnCreditEvent ， name = UpdateBetIndex  value = 5
        //【 MessageDispatcher 发送消息】：eventName = OnCreditEvent ， name = UpdateBetCredit  value = 1600
        //【 MessageDispatcher 发送消息】：eventName = OnCreditEvent ， name = UpdatedTotalBetCredit  value = 1600
    }
    void Update()
    {
        
    }
}
