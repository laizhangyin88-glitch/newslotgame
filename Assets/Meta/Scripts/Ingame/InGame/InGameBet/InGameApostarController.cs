using Dreamteck.Splines.Primitives;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using System.Collections.Generic;
using UnityEngine;

public class InGameApostarController : MonoBehaviour
{
    private MessageDelegates creditDelegates;

    ContextTextMeshProUGUI cxeText;
    //IContextText cxeText;
    //ContextElement cxeText;
    ContextElement cxeRoot;
    private void Start()
     {

        cxeRoot = transform.GetComponent<ContextElement>();
        cxeRoot.UpdateContext(true);
        //cxeText = ContextUtils.FindElement(cxeRoot, "Text Apostar", ContextSearchingType.ChildrenSearch) as IContextText;
        //cxeText = ContextUtils.FindElement(cxeRoot, "Text Apostar", ContextSearchingType.ChildrenSearch) as ContextElement;
        cxeText = ContextUtils.FindElement(cxeRoot, "Text Apostar", ContextSearchingType.ChildrenSearch) as ContextTextMeshProUGUI;
        creditDelegates = new MessageDelegates
         (
             new Dictionary<string, MessageDispatcher.EventDelegate>
             {
                 { "UpdatedTotalBetCredit",UpdatedTotalBetCredit},
             }
         );
         MessageDispatcher.Register("OnCreditEvent", creditDelegates.Delegate);

        UpdatedTotalBetCredit();
     }
     private void OnDestroy()
     {
        MessageDispatcher.UnRegister("OnCreditEvent", creditDelegates.Delegate);
     }
     private void UpdatedTotalBetCredit(EventData eventData = null)
     {
        long totalBetCredit = 0;
        if (eventData != null && eventData.value != null)
        {
            totalBetCredit = (long)(eventData.value);
        }
        else
        {
            totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit").value;
        }
        int selectLine = BlackboardUtils.FindVariable<int>("./gameNew/selectLine").value;
        cxeText.SetText($"{(totalBetCredit/ selectLine).ToString("N0")}"); 
		//ContextUtils.SetGlobalText((ContextElement)cxeText, "TEXT_BET_CREDIT", totalBetCredit / selectLine);
     }


    public void OnApostarClick()
    {

        Variable<int> betIndex = BlackboardUtils.FindVariable<int>("./betIndex");
        Variable<List<long>>  betListBase = BlackboardUtils.FindVariable<List<long>>("./gameNew/betListBase");

        betIndex.value++;
        if (betIndex.value >= betListBase.value.Count)
            betIndex.value = 0;

        Variable<List<long>> betList = BlackboardUtils.FindVariable<List<long>>("./betList");

        long totalBetCredit = betList.value[betIndex.value];

        ContextUtils.SetGlobalText(cxeText, "TEXT_BET_CREDIT", betListBase.value[betIndex.value]);

        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdateBetCredit", totalBetCredit));
        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdatedTotalBetCredit", totalBetCredit));
    }
}
