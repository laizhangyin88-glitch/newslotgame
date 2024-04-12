using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSelectBorder : MonoBehaviour
{
    // Start is called before the first frame update
    //public string name = "";

    public int index = -1;

    public GameObject selectBorder;
    void Start()
    {
        if(selectBorder == null)
            selectBorder = transform.Find("Selected")?.gameObject;

        if (index == 0)
        {
            selectBorder.SetActive(true);
        }
        else
        {
            selectBorder.SetActive(false);
        }
       // MessageDispatcher.Register("OnMachineSelectEvent", OnMachineCustomEvent);
       // MessageDispatcher.Register("OnMachineSelectIndexEvent", OnMachineCustomEvent);
    }

    private void OnDestroy()
    {
       // MessageDispatcher.UnRegister("OnMachineSelectEvent", OnMachineCustomEvent);
       // MessageDispatcher.Register("OnMachineSelectIndexEvent", OnMachineCustomEvent);
    }

   /* void OnMachineCustomEvent(ParadoxNotion.EventData eventData)
    {
        selectBorder.SetActive(false);
        if (eventData.name == "SelectItem" && (int)eventData.value == index) { 
                selectBorder.SetActive(true);
        }
    }
    void OnMachineSelectIndexEvent(ParadoxNotion.EventData eventData)
    {
        if (eventData.name == "GetSelectIndex" && index != -1)
        {
            MessageDispatcher.Dispatch("OnMachineSelectIndexEvent", new EventData<int>("ReturnSelectIndex", index));
        }
    }
   */



    // Update is called once per frame
    void Update()
    {
        
    }
}
