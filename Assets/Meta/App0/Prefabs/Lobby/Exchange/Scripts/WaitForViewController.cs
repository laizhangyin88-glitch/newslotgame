using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForViewController : MonoBehaviour
{
    private void Awake()
    {
        MessageDispatcher.Register(EVTType.ON_CONTENT_EVENT, OnListenerClose);
    }

    private void OnListenerClose(EventData eventData)
    {
        if(eventData != null && eventData.name == "CloseWaitForView")
        {
            Close();
        }
    }
    public void Close()
    {
        PopupManager.Instance.Close(this.gameObject);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(EVTType.ON_CONTENT_EVENT, OnListenerClose);
    }
}
