using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;

// using UnityEngine.Profiling;

using SlotMaker;

namespace BagelCode
{

public class UpdateCoins : MonoBehaviour
{
    public ContextElement textElement;

    public long unitCoins = 0L;
    public long currentCoins = 0L;
    public long targetCoins = 0L;

    public float minTime = 0f;
    public float maxTime = 0f;
    public Vector2 limitTime = Vector2.zero;

    public string key;
    public StringTable.StringTableType tableType;
    public IContextText property;

    public bool useDebug = false;

    public string completeEventName;

    public float progress = 0f;

    private long oldTargetCoins = 0L;
    private long deltaCoins = 0L;

    private float cA = 0f;
    private float cB = 0f;
    private const float cMinCoinsRate = 1f;
    private const float cMaxCoinsRate = 10f;

    private long prevCoins = 0;

    public void Init(   ContextElement targetText,
                        long newUnitCoins,
                        long newCurrentCoins,
                        long newTargetCoins,
                        float newMinTime,
                        float newMaxTime,
                        Vector2 newLimitTime,
                        string newStringKey,
                        StringTable.StringTableType newTableType,
                        string newCompleteEventName)
    {
        textElement = targetText;
        if(textElement != null)
            property = textElement as IContextText;

        unitCoins = newUnitCoins;
        currentCoins = newCurrentCoins;
        targetCoins = newTargetCoins;

        minTime = newMinTime;
        maxTime = newMaxTime;
        limitTime = newLimitTime;

        key = newStringKey;
        tableType = newTableType;

        completeEventName = newCompleteEventName;

        UpdateCoefficient();

        SetText(currentCoins, true);
        if(currentCoins != targetCoins)
        {
            SetTarget(targetCoins, false);
        }
    }

    public void SetTarget(  long newTargetCoins,
                            bool isForceUpdate)
    {
        targetCoins = newTargetCoins;

        if(isForceUpdate)
        {
            currentCoins = newTargetCoins;
            SetText(currentCoins, isForceUpdate);
        }
    }

    private void UpdateCoefficient()
    {
        // float ca = ((cMaxCoinsRate / maxTime) - (cMinCoinsRate / minTime)) / (maxTime - minTime);
        // float cb = (cMinCoinsRate / minTime) - ca * minTime;
        cA = (maxTime - minTime) / (cMaxCoinsRate - cMinCoinsRate);
        cB = minTime - cA * cMinCoinsRate;
    }

    private void SetText(long coins, bool isForceUpdate)
    {
        if(property == null) return;
        if(!isForceUpdate && prevCoins == coins) return;

        bool error = true;
        property.SetText(StringTableUtils.GetString(tableType, key, coins, out error));

        prevCoins = coins;
    }

    private void Update()
    {
        long diffCoins = targetCoins - currentCoins;
        if (diffCoins == 0)
            return;

        // UnityEngine.Profiling.Profiler.BeginSample("UpdateCoins.Update");

        if (useDebug)
            UpdateCoefficient();

        if (currentCoins == 0L || targetCoins != oldTargetCoins)
        {
            float diffCoinsRate = (float)diffCoins / (float)unitCoins;
            float deltaTime = Mathf.Clamp(cA * diffCoinsRate + cB, limitTime.x, limitTime.y);
            deltaCoins = (long)(diffCoinsRate / deltaTime * (float)unitCoins);

            if (useDebug)
                Debug.Log("Expected animation time: " + deltaTime);

            oldTargetCoins = targetCoins;
        }

        currentCoins += (long)Mathf.Max(((float)deltaCoins * Time.deltaTime), 1f);
        if (currentCoins >= targetCoins)
        {
            currentCoins = targetCoins;
            progress = 1f;

            if (!string.IsNullOrEmpty(completeEventName))
                Graph.SendGlobalEvent(new EventData(completeEventName), null);
        }
        else
        {
            progress = (float)currentCoins / (float)targetCoins;
        }

        SetText(currentCoins, false);

        // UnityEngine.Profiling.Profiler.EndSample();
    }
}

}
