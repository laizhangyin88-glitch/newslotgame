using ParadoxNotion;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

public class InGameApostarController : MonoBehaviour
{
    private MessageDelegates creditDelegates;

    //ContextTextMeshProUGUI cxeText;
    IContextText cxeText;
    ContextElement cxeRoot;
    private void Start()
     {

        cxeRoot = transform.GetComponent<ContextElement>();
        cxeRoot.UpdateContext(true);
        cxeText = ContextUtils.FindElement(cxeRoot, "Text Apostar", ContextSearchingType.ChildrenSearch) as IContextText;

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
     }

}
