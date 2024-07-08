using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSA_JackpotNum : OSA<JackpotNumParams, JackpotNumItemViewsHolder>
{
    public AnimationCurve curve = new AnimationCurve(
    new Keyframe(1f, 1f, 1f, 1f)
    //new Keyframe(0.06f, 0.7f, 4.778541f, 4.778541f),
    //new Keyframe(0.2f, 1.0f, 0.0f, 0.0f),
    //new Keyframe(0.3f, 1.0f, -0.02080777f, -0.02080777f),
    //new Keyframe(0.8f, 0.1f, -0.3684081f, -0.3684081f),
    //new Keyframe(1.0f, 0.1f, -1.041666f, -1.041666f)
);

    [SerializeField]
    private List<Sprite> sprites;

    private float middleY;
    private float totalHeight;
    private float centerWeight = 0.1f;

    private int preItemIndex;
    [HideInInspector]
    public int curItemIndex;

    [HideInInspector]
    public int numIndex;
    [HideInInspector]
    public string flag;

    public Coroutine scrollCoroutine = null;

    protected override void Start()
    {
        base.Start();
        middleY = GetComponent<RectTransform>().sizeDelta.y * -.5f;
        ResetItems(10);
        totalHeight = _Params.DefaultItemSize * 10.75f;
    }

    protected override void Update()
    {
        base.Update();
        var middleVH = _Params.Snapper.GetMiddleVH(out _) as JackpotNumItemViewsHolder;
        //Debug.Log(_Params.GetItemValueAtIndex(middleVH.ItemIndex));
        curItemIndex = _Params.GetItemValueAtIndex(middleVH.ItemIndex);
        if (preItemIndex == 9 && curItemIndex == 0)
            MessageDispatcher.Dispatch("JackpotNumChange", new EventData<int>(flag, numIndex));
        preItemIndex = curItemIndex;
    }

    protected override JackpotNumItemViewsHolder CreateViewsHolder(int itemIndex)
    {
        var item = new JackpotNumItemViewsHolder();
        item.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        //prefab.SetActive(false);
        return item;
    }

    protected override void UpdateViewsHolder(JackpotNumItemViewsHolder newOrRecycled)
    {
        newOrRecycled.image.sprite = sprites[newOrRecycled.ItemIndex];
    }

    public void Simulation(int targetIndex, float animationTime, int loopCount)
    {
        StopScorllCoroutine();
        scrollCoroutine = StartCoroutine(SimulationCoroutine(targetIndex, animationTime, loopCount));
    }

    private System.Collections.IEnumerator SimulationCoroutine(int targetIndex, float animationTime, int loopCount)
    {
        _Params.effects.LoopItems = true;
        _Params.effects.InertiaDecelerationRate = 0f;

        float targetHeight = 0.0f;
        JackpotNumItemViewsHolder currentItem = GetCurrentItem();
        targetHeight += GetTargetDistance(currentItem, targetIndex);
        targetHeight += loopCount * totalHeight;
        float timeScaleResult = GetCurveTimeScaleResult(animationTime);
        float velocityMultiplier = targetHeight / timeScaleResult;
        int lastItemIndex = curItemIndex;// currentItem.ItemIndex;
        float deltaTime = 0.0f;
        bool spinDuration = false;
        while (deltaTime < animationTime)
        {
            deltaTime += UnityEngine.Time.deltaTime;
            UpdateVelocity(velocityMultiplier * curve.Evaluate(deltaTime / animationTime) * 1.0f);

            currentItem = GetCurrentItem();
            if (currentItem != null && currentItem.ItemIndex != lastItemIndex)
            {
                // tick
                lastItemIndex = currentItem.ItemIndex;
            }
            if (spinDuration == false && deltaTime * 2.0f > animationTime)
            {
                spinDuration = true;
            }
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(true);
        _Params.effects.InertiaDecelerationRate = 0.8f;
        float duration = 0.25f;
        if (CheckItemIsTarget(currentItem, targetIndex) && (currentItem.IsCenter(middleY, _Params.DefaultItemSize * centerWeight) || currentItem.GetPosY() <= middleY))
        {
            // target arrive
            SmoothScrollTo(targetIndex, duration, 0.5f, 0.5f,onDone: () => { StopScorllCoroutine(); });
        }
        else
        {
            // set last wheel velocity
            float lastVelocity = velocityMultiplier * curve.Evaluate(1.0f) * 1.0f;
            int checkTargetIndex = targetIndex;
            // move to target index wait
            while (true)
            {
                UpdateVelocity(lastVelocity);
                yield return new WaitForFixedUpdate();
                currentItem = GetCurrentItem();
                if (CheckItemIsTarget(currentItem, checkTargetIndex) && (currentItem.IsCenter(middleY, _Params.DefaultItemSize * centerWeight) || currentItem.GetPosY() <= middleY))
                {
                    SmoothScrollTo(checkTargetIndex, duration, 0.5f, 0.5f, onDone: () => { StopScorllCoroutine(); });
                }
                else if (currentItem != null && currentItem.ItemIndex != lastItemIndex)
                {
                    // tick
                    lastItemIndex = currentItem.ItemIndex;
                    //checkTargetIndex = lastItemIndex;
                }
            }
        }
    }

    private bool StopScorllCoroutine()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            StopMovement();
            scrollCoroutine = null;
            return true;
        }
        return false;
    }

    private JackpotNumItemViewsHolder GetCurrentItem()
    {
       
        for (int i = 0; i < VisibleItemsCount; ++i)
        {
            if (_VisibleItems[i].IsCurrentItem(middleY))
                return _VisibleItems[i];
        }
        return null;
    }

    private float GetTargetDistance(JackpotNumItemViewsHolder currentItem, int targetIndex)
    {
        if (currentItem == null)
            return 0.0f;

        int currentItemIndex = currentItem.ItemIndex;
        float distHeight = 0;
        if (currentItemIndex != targetIndex)
        {
            if (currentItemIndex > targetIndex) // ex) 5 -> 2
                distHeight = (10 - currentItemIndex + targetIndex) * _Params.DefaultItemSize; 
            else // ex) 5 -> 16
                //distHeight = (targetIndex - currentItemIndex + 0.75f) * _Params.DefaultItemSize;
                distHeight = (targetIndex - currentItemIndex) * _Params.DefaultItemSize;
        }
        else
        {
            //distHeight = currentItem.GetPosY() - middleY;
            //if (distHeight < 0.0f)
                distHeight += totalHeight;// + 0.5f * _Params.DefaultItemSize;
        }
        return distHeight;
    }

    private float GetCurveTimeScaleResult(float animationTime)
    {
        // Animation Curve Distance
        float tempDeltaTimeWeight = 50.0f;// 1.0f / Time.deltaTime;
        int precisionStep = Mathf.RoundToInt(tempDeltaTimeWeight * animationTime);

        float calcMoveHeightScale = 0.0f;
        float stepSize = 1.0f / Convert.ToSingle(precisionStep);
        float currTime = 0.0f;
        for (int i = 0; i < precisionStep; ++i)
        {
            currTime += stepSize;
            calcMoveHeightScale += curve.Evaluate(currTime);
        }
        calcMoveHeightScale *= stepSize;
        return animationTime * calcMoveHeightScale;
    }

    private void UpdateVelocity(float value)
    {
        Vector2 updateVelocity = Velocity;
        updateVelocity.y = value;
        Velocity = updateVelocity;
    }

    private bool CheckItemIsTarget(JackpotNumItemViewsHolder checkItem, int targetIndex)
    {
        return checkItem != null && checkItem.ItemIndex == targetIndex;
    }
}

[Serializable] // serializable, so it can be shown in inspector
public class JackpotNumParams : BaseParamsWithPrefab
{
    public int startItemNumber = 0;
    public int increment = 1;

    /// <summary>The value of each item is calculated dynamically using its <paramref name="index"/>, <see cref="startItemNumber"/> and the <see cref="increment"/></summary>
    /// <returns>The item's value (the displayed number)</returns>
    public int GetItemValueAtIndex(int index) { return startItemNumber + increment * index; }
}

public class JackpotNumItemViewsHolder : BaseItemViewsHolder
{
    public Image image;

    public override void CollectViews()
    {
        base.CollectViews();
        image = root.GetComponent<Image>();
    }

    public bool IsCurrentItem(float centerY)
    {
        float height = root.sizeDelta.y * 0.5f;
        return root.anchoredPosition.y + height >= centerY && root.anchoredPosition.y - height <= centerY;
    }

    public float GetPosY()
    {
        return root.anchoredPosition.y;
    }

    public bool IsCenter(float centerY, float centerWeight)
    {
        return root.anchoredPosition.y + centerWeight >= centerY && root.anchoredPosition.y - centerWeight <= centerY;
    }
}
