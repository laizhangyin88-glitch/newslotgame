using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;

// using UnityEngine.Profiling;

using SlotMaker;

namespace BagelCode
{

    public class UpdateNaviUserCoins : MonoBehaviour
    {
        public ContextElement textElement;

        public long unitCoins = 0L;
        public long unitPowCoins = 0L;
        public long currentCoins = 0L;
        public long targetCoins = 0L;

        public float minTime = 0f;
        public float maxTime = 0f;
        public Vector2 limitTime = Vector2.zero;

        public string defaultFormatKey;
        public string simpleFormatKey;
        public StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        public IContextText property;

        public bool useDebug = false;

        public float progress = 0f;

        private long oldTargetCoins = 0L;
        private long deltaCoins = 0L;

        private float cA = 0f;
        private float cB = 0f;
        private const float cMinCoinsRate = 1f;
        private const float cMaxCoinsRate = 10f;

        private long prevCoins = 0;

        private int powNumber = 0;
        private long simplePowCoin = 0;

        private ClientModels.Orientation orientation = ClientModels.Orientation.UNKNOWN;
        private bool isInGame = false;

        public void Init(ContextElement targetText,
                            long newUnitCoins,
                            long newCurrentCoins,
                            long newTargetCoins,
                            float newMinTime,
                            float newMaxTime,
                            Vector2 newLimitTime,
                            string newDefaultFormatKey,
                            string newSimpleFormatKey)
        {
            textElement = targetText;
            if (textElement != null)
                property = textElement as IContextText;

            unitCoins = newUnitCoins;
            currentCoins = newCurrentCoins;
            targetCoins = newTargetCoins;

            minTime = newMinTime;
            maxTime = newMaxTime;
            limitTime = newLimitTime;

            defaultFormatKey = newDefaultFormatKey;
            simpleFormatKey = newSimpleFormatKey;

            powNumber = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/misc/PORTRAIT_POW_NUMBER").value;
            if (powNumber > 0)
            {
                simplePowCoin = (long)Math.Pow(10, powNumber + 1);
                unitPowCoins = Math.Max(simplePowCoin / 10, unitCoins);
            }

            var enterGameInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "enterGameInfo");

            if (enterGameInfo != null && enterGameInfo.value != null)
                orientation = enterGameInfo.value.GetValue<ClientModels.Orientation>("orientation");

            isInGame = MainBlackboard.Get().GetValue<bool>("inGame");

            UpdateCoefficient();

            SetText(currentCoins, true);
            if (currentCoins != targetCoins)
            {
                SetTarget(targetCoins, false);
            }
        }

        public void SetTarget(long newTargetCoins,
                                bool isForceUpdate)
        {
            if (false == BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isPossibleUpdateNavi").value) return;

            targetCoins = newTargetCoins;

            if (isForceUpdate)
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
            if (property == null) return;
            if (!isForceUpdate && prevCoins == coins) return;

            if (UseSimpleFormat())
            {
                Debug.LogError("更新金币...................................................1   " + coins);
                property.SetText(StringTableUtils.GetString(tableType, simpleFormatKey, coins));
            }
            else
            {
                Debug.LogError("更新金币...................................................2   " + coins);
                property.SetText(StringTableUtils.GetString(tableType, defaultFormatKey, coins));
            }
            prevCoins = coins;
        }

        private void Update()
        {
            long diffCoins = targetCoins - currentCoins;
            if (diffCoins == 0)
                return;

            if (useDebug)
                UpdateCoefficient();

            if (currentCoins == 0L || targetCoins != oldTargetCoins)
            {
                double diffCoinsRate = (double)diffCoins / (UseSimpleFormat() ? (double)unitPowCoins : (double)unitCoins);
                double deltaTime = (double)cA * diffCoinsRate + (double)cB;
                if (deltaTime < (double)limitTime.x)
                    deltaTime = (double)limitTime.x;
                else if (deltaTime > (double)limitTime.y)
                    deltaTime = (double)limitTime.y;

                deltaCoins = (long)(diffCoinsRate / deltaTime * (UseSimpleFormat() ? (double)unitPowCoins : (double)unitCoins));

                if (useDebug)
                    Debug.Log("Expected animation time: " + deltaTime);

                oldTargetCoins = targetCoins;
            }

            currentCoins += (long)Math.Max(((double)deltaCoins * (double)Time.deltaTime), 1);
            if (currentCoins >= targetCoins)
            {
                currentCoins = targetCoins;
                progress = 1f;
            }
            else
            {
                progress = (float)((double)currentCoins / (double)targetCoins);
            }

            SetText(currentCoins, false);
        }

        private bool UseSimpleFormat()
        {
            if (!OrientationUtils.Instance.PossibleChangeOrientation())
                return false;

            if (!isInGame)
                return false;

            return orientation == ClientModels.Orientation.PORTRAIT && powNumber > 0 && currentCoins >= simplePowCoin;
        }
    }
}
