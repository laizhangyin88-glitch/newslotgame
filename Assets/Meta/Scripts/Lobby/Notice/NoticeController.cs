using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

public class NoticeController : MonoBehaviour
{
    private Blackboard rootBB;
    private ContextElement rootElement;

    private const string CLICK_BUTTON_NEXT_SEND_ELEMENT_NAME = "Button Next";
    private const string CLICK_BUTTON_NEXT_SEND_KEY = "BUTTON_NEXT";
    private const string CLICK_BUTTON_NEXT_SEND_EVENT_NAME = "Next";

    private bool isInit = false;

    public void Initialize()
    {
        if (isInit) return;

        rootBB = gameObject.GetComponent<Blackboard>();
        rootElement = gameObject.GetComponent<ContextElement>();

        rootElement.UpdateContext(true);
        SetButtonClickEvent();

        isInit = true;
    }

    private void SetButtonClickEvent()
    {

    }

}
