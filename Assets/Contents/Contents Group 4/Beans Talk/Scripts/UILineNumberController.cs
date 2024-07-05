using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILineNumberController : MonoBehaviour
{
    private MessageDelegates creditDelegates;
    private void Start()
    {
        creditDelegates = new MessageDelegates
         (
             new Dictionary<string, MessageDispatcher.EventDelegate>
             {
                 { "Line",OnLineChange},
             }
         );
        MessageDispatcher.Register("OnCreditEvent", creditDelegates.Delegate);

        OnLineChange();
    }
    private void OnLineChange(EventData eventData = null)
    {
        int line;
        if (eventData != null && eventData.value != null)
        {
            line = (int)(eventData.value);
        }
        else
        {
            line = BlackboardUtils.FindVariable<int>("./gameNew/selectLine").value;
        }
        for (int i =0;i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(i < line);
        }
    }
    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnCreditEvent", creditDelegates.Delegate);
    }
}
