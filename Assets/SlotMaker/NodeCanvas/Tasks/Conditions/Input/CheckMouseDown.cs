using UnityEngine;
using UnityEngine.EventSystems;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/GameObject")]
public class CheckMouseDown : ConditionTask<Transform>
{
    public BBParameter<float> delay;

    private bool isMouseDown = false;
    private float mouseDownTime = 0f;

    private const string ON_POINTER_DOWN = "OnPointerDown";
    private const string ON_POINTER_UP = "OnPointerUp";
    private const string ON_POINTER_EXIT = "OnPointerExit";

    protected override string info
    {
        get
        {
            if (!isMouseDown)
                return "MouseDown 0 sec";

            return string.Format("MouseDown {0} sec", (Time.unscaledTime - mouseDownTime));
        }
    }

    protected override string OnInit()
    {
        RegisterEvent(ON_POINTER_DOWN);
        RegisterEvent(ON_POINTER_UP);
        RegisterEvent(ON_POINTER_EXIT);
        return null;
    }

    protected override bool OnCheck()
    {
        if (!isMouseDown)
            return false;

        return (Time.unscaledTime - mouseDownTime) > delay.value;
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        isMouseDown = true;
        mouseDownTime = Time.unscaledTime;
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        isMouseDown = false;
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        isMouseDown = false;
    }
}

}
