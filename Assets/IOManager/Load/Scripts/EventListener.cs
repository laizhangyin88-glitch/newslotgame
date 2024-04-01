using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action onclick;
    public Action onPointDown;
    public Action onPointUp;

    private float durationThreshold = 0.2f;
    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    void Update()
    {
          //按下计时
        if ( isPointerDown && !longPressTriggered)
        {
            if (Time.time - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
            }
        }
    }

    //按下-放开-间隔时间不超过2秒
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!longPressTriggered && !isPointerDown )
        {
            onclick?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;

        onPointDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointUp?.Invoke();
        isPointerDown = false;
    }

    public static EventListener AddEventListenr(GameObject obj)
    {
        EventListener listener = obj.GetComponent<EventListener>();
        if (listener == null)
        {
            listener = obj.AddComponent<EventListener>();
        }

        return listener;
    }

    public static void RemoveEventListener(GameObject obj)
    {
        EventListener listener = obj.GetComponent<EventListener>();
        if (listener == null) return;
        GameObject.Destroy(listener);
    }
}
