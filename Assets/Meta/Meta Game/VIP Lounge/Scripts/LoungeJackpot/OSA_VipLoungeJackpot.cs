using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using BagelCode.VipLounge;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_VipLoungeJackpot : OSA<VipLoungeJackpotItemParams, VipLoungeJackpotItemViewHolder>
    {
        private Blackboard bb;
        private Blackboard loungeJackpotInfo;
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 0.0f, 0.0f),
            new Keyframe(0.06f, 0.7f, 4.778541f, 4.778541f),
            new Keyframe(0.2f, 1.0f, 0.0f, 0.0f),
            new Keyframe(0.3f, 1.0f, -0.02080777f, -0.02080777f),
            new Keyframe(0.8f, 0.1f, -0.3684081f, -0.3684081f),
            new Keyframe(1.0f, 0.1f, -1.041666f, -1.041666f)
        );

        private long baseWinCredit = 0L;
        private bool isAppear = false;
        private float middleY = 0.0f;
        private float totalHeight = 0;
        private List<int> sameTargetIndexList = new List<int>();

        public Coroutine spinCoroutine = null;
        public float centerWeight = 0.1f;


        protected override void Start()
        {
            base.Start();
            OnInit();
        }

        private void OnInit()
        {
            OnInitData();
            CreateItemList();
            middleY = GetComponent<RectTransform>().sizeDelta.y * -0.5f;
        }

        public void SetMainBlackboard(Blackboard mainBB)
        {
            bb = mainBB;
            loungeJackpotInfo = bb.GetValue<Blackboard>("loungeJackpotInfo");
        }

        private void OnInitData()
        {
            baseWinCredit = loungeJackpotInfo?.GetValue<long>("baseWinCredit") ?? 0L;
        }

        private void CreateItemList()
        {
            List<LoungeJackpotWheelItem> dataList = GetLoungeJackpotWheelItems();
            if (dataList == null || dataList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(dataList);
            _Params.effects.LoopItems = true;

            ResetItems(dataList.Count);
            ScrollTo(GetGrandIndex(dataList), 0.5f, 0.5f);
        }

        protected override VipLoungeJackpotItemViewHolder CreateViewsHolder(int itemIndex)
        {
            VipLoungeJackpotItemViewHolder viewHolder = new VipLoungeJackpotItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(_Params.data[itemIndex].winType), _Params.Content, itemIndex);
            return viewHolder;
        }

        protected override void UpdateViewsHolder(VipLoungeJackpotItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex, baseWinCredit, isAppear);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            {
                newOrRecycled.MarkForRebuild();
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override bool IsRecyclable(VipLoungeJackpotItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].winType);
        }

        public override void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            ClearVisibleItems();
            base.ResetItems(itemsCount, contentPanelEndEdgeStationary, keepVelocity);
        }

        private bool CheckItemIsTarget(VipLoungeJackpotItemViewHolder checkItem, int targetIndex)
        {
            return checkItem != null && checkItem.ItemIndex == targetIndex;
        }

        private float GetItemHeight(List<LoungeJackpotWheelItem> itemList)
        {
            if (itemList == null) return 0;

            float totalHeight = 0.0f;
            for (int i = 0; i < itemList.Count; ++i)
            {
                float itemHeight = GetItemHeight(itemList[i].winType);
                totalHeight += itemHeight;
            }
            return totalHeight;
        }

        private float GetItemHeight(LoungeJackpotWinType winType)
        {
            switch (winType)
            {
                case LoungeJackpotWinType.CREDIT:
                    return 50.0f;
                case LoungeJackpotWinType.MINI:
                    return 76.0f;
                case LoungeJackpotWinType.MINOR:
                    return 90.0f;
                case LoungeJackpotWinType.MAJOR:
                    return 104.0f;
                case LoungeJackpotWinType.GRAND:
                    return 120.0f;
                default:
                    return 0.0f;
            }
        }

        private List<LoungeJackpotWheelItem> GetLoungeJackpotWheelItems()
        {
            List<LoungeJackpotWheelItem> dataList = new List<LoungeJackpotWheelItem>();
            List<Blackboard> wheelPreset = VipLounge.VipLounge.Utils.LoungeJackpotWheelPreset(bb);
            if (wheelPreset == null)
                wheelPreset = new List<Blackboard>();

            int presetCount = wheelPreset.Count;
            for (int i = 0; i < presetCount; ++i)
            {
                LoungeJackpotWheelItem item = new LoungeJackpotWheelItem()
                {
                    winType = wheelPreset[i].GetValue<LoungeJackpotWinType>("winType"),
                    minMultiplierNumerator = wheelPreset[i].GetValue<long>("minMultiplierNumerator"),
                    minValue = wheelPreset[i].GetValue<long>("minValue")
                };
                totalHeight += GetItemHeight(item.winType);
                dataList.Add(item);
            }

            return dataList;
        }

        private int GetGrandIndex(List<LoungeJackpotWheelItem> dataList)
        {
            if (dataList != null && dataList.Count > 0)
            {
                for(int i = 0; i < dataList.Count; ++i)
                {
                    if (dataList[i].winType == LoungeJackpotWinType.GRAND)
                        return i;
                }
            }
            return 0;
        }

        private VipLoungeJackpotItemViewHolder GetCurrentItem()
        {
            for (int i = 0; i < VisibleItemsCount; ++i)
            {
                if (_VisibleItems[i].IsCurrentItem(middleY) == true)
                    return _VisibleItems[i];
            }
            return null;
        }

        private float GetTargetDistance(VipLoungeJackpotItemViewHolder currentItem, int targetIndex)
        {
            if (currentItem == null)
                return 0.0f;

            int currentItemIndex = currentItem.ItemIndex;
            float distHeight = 0.0f;
            List<LoungeJackpotWheelItem> checkHeightList = new List<LoungeJackpotWheelItem>();
            if (currentItemIndex != targetIndex)
            {
                if (currentItemIndex > targetIndex) // ex) 5 -> 2
                {
                    for (int i = targetIndex; i <= currentItemIndex; ++i)
                        checkHeightList.Add(_Params.data[i]);
                }
                else // ex) 5 -> 16
                {
                    int paramCount = _Params.data.Count;
                    int checkCount = currentItemIndex + paramCount;
                    for (int i = targetIndex; i <= checkCount; ++i)
                        checkHeightList.Add(_Params.data[i % paramCount]);
                }
                distHeight = GetItemHeight(checkHeightList);
                float targetCenterHeight = GetItemHeight(_Params.data[targetIndex].winType) * 0.5f;
                float currentCenterHeight = currentItem.GetHeight() * 0.5f + (middleY - currentItem.GetPosY());
                //Debug.Log(string.Format("TEst ::: h : {0} // c : {1} / {2} // t : {3} / {4}", distHeight, currentItemIndex, currentCenterHeight, targetIndex, targetCenterHeight));
                distHeight -= targetCenterHeight + currentCenterHeight;
            }
            else
            {
                distHeight = currentItem.GetPosY() - middleY;
                if (distHeight < 0.0f)
                    distHeight += totalHeight;
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

        private void SetTargetIndexList(int targetIndex)
        {
            sameTargetIndexList.Clear();
            int count = _Params.data.Count;
            if (count > 0 && count > targetIndex && targetIndex >= 0)
            {
                long targetMinValue = _Params.data[targetIndex].minValue;
                for (int i = 0; i < count; ++i)
                {
                    if (targetMinValue == _Params.data[i].minValue)
                        sameTargetIndexList.Add(i);
                }
            }
        }

        public void Simulation(int targetIndex, float animationTime, int loopCount, System.Action endCallback = null, System.Action spinDurationCallback = null, System.Action successCallback = null)
        {
            if (StopSpinCoroutine())
                return;
            spinCoroutine = StartCoroutine(SimulationCoroutine(targetIndex, animationTime, loopCount, endCallback, spinDurationCallback, successCallback));
        }

        private IEnumerator SimulationCoroutine(int targetIndex, float animationTime, int loopCount, System.Action endCallback = null, System.Action spinDurationCallback = null, System.Action successCallback = null)
        {
            isAppear = true;
            _Params.effects.LoopItems = true;
            _Params.effects.InertiaDecelerationRate = 0.0f;

            SetTargetIndexList(targetIndex);

            float targetHeight = 0.0f;
            // Target <-> Current distance
            VipLoungeJackpotItemViewHolder currentItem = GetCurrentItem();
            targetHeight += GetTargetDistance(currentItem, targetIndex);
            targetHeight += loopCount * totalHeight;
            // Animation Curve Distance
            float timeScaleResult = GetCurveTimeScaleResult(animationTime);

            float velocityMultiplier = targetHeight / timeScaleResult;
            int lastItemIndex = currentItem.ItemIndex;
            // Run Spin
            GSManager.Instance.GetHandler(VipLounge.VipLounge.Defines.LOUNGE_JACKPOT_WHEEL).Play();
            float deltaTime = 0.0f;
            bool spinDuration = false;
            while (deltaTime < animationTime)
            {
                deltaTime += UnityEngine.Time.deltaTime;
                UpdateVelocity(velocityMultiplier * curve.Evaluate(deltaTime / animationTime) * -1.0f);

                currentItem = GetCurrentItem();
                if (currentItem != null && currentItem.ItemIndex != lastItemIndex)
                {
                    // tick
                    lastItemIndex = currentItem.ItemIndex;
                    GSManager.Instance.GetHandler(VipLounge.VipLounge.Defines.LOUNGE_JACKPOT_WHEEL_TICK).Play();
                }
                if (spinDuration == false && deltaTime * 2.0f > animationTime)
                {
                    if (spinDurationCallback != null)
                        spinDurationCallback();
                    spinDuration = true;
                }
                yield return new WaitForFixedUpdate();
            }

            _Params.effects.InertiaDecelerationRate = 0.8f;
            float duration = 0.1f;
            if (CheckItemIsTarget(currentItem, targetIndex) && (currentItem.IsCenter(middleY, currentItem.GetHeight() * centerWeight) || currentItem.GetPosY() <= middleY))
            {
                // target arrive
                SmoothScrollTo(targetIndex, duration, 0.5f, 0.5f);
                yield return new WaitForSeconds(duration);
            }
            else
            {
                // set last wheel velocity
                float lastVelocity = velocityMultiplier * curve.Evaluate(1.0f) * -1.0f;
                int checkTargetIndex = targetIndex;
                // move to target index wait
                while (true)
                {
                    UpdateVelocity(lastVelocity);
                    yield return new WaitForFixedUpdate();
                    currentItem = GetCurrentItem();
                    if (CheckItemIsTarget(currentItem, checkTargetIndex) && (currentItem.IsCenter(middleY, currentItem.GetHeight() * centerWeight) || currentItem.GetPosY() <= middleY))
                    {
                        SmoothScrollTo(checkTargetIndex, duration, 0.5f, 0.5f);
                        yield return new WaitForSeconds(duration);
                        break;
                    }
                    else if (currentItem != null && currentItem.ItemIndex != lastItemIndex)
                    {
                        // tick
                        lastItemIndex = currentItem.ItemIndex;
                        GSManager.Instance.GetHandler(VipLounge.VipLounge.Defines.LOUNGE_JACKPOT_WHEEL_TICK).Play();
                        if (sameTargetIndexList.Contains(lastItemIndex))
                            checkTargetIndex = lastItemIndex;
                    }
                }
            }

            GSManager.Instance.GetHandler(VipLounge.VipLounge.Defines.LOUNGE_JACKPOT_WHEEL_STOP).Play();
            bool isItemActiveEnded = false;
            currentItem.SetCallback(() => { isItemActiveEnded = true; });
            currentItem.SetTriggerAnimation("isActive");
            StopMovement();
            if (endCallback != null)
                endCallback();
            yield return new WaitUntil(() => isItemActiveEnded);
            if (successCallback != null)
                successCallback();
        }

        [Button]
        private void TestSimulation(int targetIndex = 0, float animationTime = 4.0f, int loopCount = 1)
        {
            if (StopSpinCoroutine())
                return;

            spinCoroutine = StartCoroutine(SimulationCoroutine(targetIndex, animationTime, loopCount));
        }

        private bool StopSpinCoroutine()
        {
            if (spinCoroutine != null)
            {
                StopCoroutine(spinCoroutine);
                StopMovement();
                spinCoroutine = null;
                return true;
            }
            return false;
        }

        public void AppearAnimationExtraPoint(float animationTime)
        {
            for (int i = 0; i < VisibleItemsCount; ++i)
                _VisibleItems[i].AppearAnimationExtraPoint(animationTime);
        }

        [Button]
        private void TestGetCurrentItem()
        {
            var currentItem = GetCurrentItem();
            Debug.Log(string.Format("Test Current Item :: {0} / {1}", currentItem.ItemIndex, currentItem.WheelData.winType));
        }

        [Button]
        private void InitReelPosition()
        {
            ScrollTo(GetGrandIndex(_Params.data), 0.5f, 0.5f);
        }
    }

    [Serializable]
    public class VipLoungeJackpotItemParams : BaseParams
    {
        public List<LoungeJackpotWheelItem> data = new List<LoungeJackpotWheelItem>();
        public List<GameObject> prefabs = new List<GameObject>();
        public LoungeJackpotWinType itemType = LoungeJackpotWinType.UNKNOWN;

        public GameObject GetPrefab(LoungeJackpotWinType idx)
        {
            itemType = idx;
            if (prefabs != null && itemType != LoungeJackpotWinType.UNKNOWN && prefabs.Count > (int)itemType - 1)
                return prefabs[(int)itemType - 1];
            return null;
        }
    }

    [Serializable]
    public class VipLoungeJackpotItemViewHolder : BaseItemViewsHolder
    {
        private VipLoungeJackpotItemCellController controller;
        private LoungeJackpotWheelItem wheelData = null;
        private long baseCredit = 0L;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }
        public LoungeJackpotWheelItem WheelData { get { return wheelData; } }

        private long GetCredit(bool isMax)
        {
            if (wheelData == null)
                return 0L;

            if (isMax)
                return VipLounge.VipLounge.Utils.GetLoungeJackpotCreditFloor(Math.Max(baseCredit, wheelData.minValue));
            else
            {
                if (baseCredit > wheelData.minValue)
                    return VipLounge.VipLounge.Utils.GetLoungeJackpotCreditFloor(wheelData.minValue);
                else
                    return 0L;
            }
        }

        public void UpdateView(LoungeJackpotWheelItem _data, int _index, long baseWinCredit, bool isMax)
        {
            wheelData = _data;
            controller.index = _index;
            baseCredit = NumberUtils.GetMultiplierNumeratorValue(baseWinCredit, wheelData.minMultiplierNumerator);

            if (wheelData.winType == LoungeJackpotWinType.CREDIT)
                controller.SetCellNumberText(GetCredit(isMax));
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<VipLoungeJackpotItemCellController>();

            ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            ContentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            if (ContentSizeFitter)
                ContentSizeFitter.enabled = true;
        }

        public bool CanPresentModelType(LoungeJackpotWinType winType)
        {
            return wheelData != null && wheelData.winType == winType;
        }

        public float GetHeight()
        {
            return controller.height;
        }

        public float GetPosY()
        {
            return controller.posY;
        }

        public bool IsCurrentItem(float centerY)
        {
            float height = controller.height * 0.5f;
            return controller.posY + height >= centerY && controller.posY - height <= centerY;
        }

        public bool IsCenter(float centerY, float centerWeight)
        {
            return controller.posY + centerWeight >= centerY && controller.posY - centerWeight <= centerY;
        }

        public void AppearAnimationExtraPoint(float animationTime)
        {
            if (wheelData.winType != LoungeJackpotWinType.CREDIT)
                return;

            controller?.AppearAnimationExtraPoint(animationTime, GetCredit(false), GetCredit(true));
        }

        public void SetTriggerAnimation(string trigger)
        {
            controller?.SetTriggerAnimation(trigger);
        }

        public void SetCallback(System.Action itemCallback)
        {
            controller?.SetCallback(itemCallback);
        }
    }
}
