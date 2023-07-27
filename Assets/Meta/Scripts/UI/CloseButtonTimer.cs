using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{

public class CloseButtonTimer : MonoBehaviour
{
    public UnityBoolEvent onButtonState;
    public UnityBoolEvent onTimerState;
    public UnityStringEvent onChangeValue;

    private bool useTimer = false;
    private float remainTime = 0f;
    private float elapsedSec = 0f;
    private float prevElapsedSec = 0f;
    private float changeValueInterval = 0.2f;
    private string currentSecText = "";

    public void StartTimer(float seconds)
    {
        onButtonState.Invoke(false);
        onTimerState.Invoke(true);
        useTimer = true;
        elapsedSec = 0f;
        prevElapsedSec = 0f;
        remainTime = seconds;
    }

    public void StopTimer()
    {
        onButtonState.Invoke(true);
        onTimerState.Invoke(false);
        useTimer = false;
    }

    private void Update()
    {
        if(useTimer)
        {
            elapsedSec += Time.deltaTime;
            remainTime -= Time.deltaTime;

            if(elapsedSec > prevElapsedSec)
            {
                prevElapsedSec = elapsedSec + changeValueInterval;
                int remainSec = (int)remainTime;
                currentSecText = System.Convert.ToString(remainSec + 1);
                onChangeValue.Invoke(currentSecText);
            }

            if(remainTime <= 0f)
            {
                StopTimer();
            }
        }
    }
}

}