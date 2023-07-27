using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using TMPro;

namespace BagelCode.GemJackpot
{
    public class GemJackpotBonusSymbol : MonoBehaviour
    {
        public GemJackpotBonusSymbolController symbolController = null;
        public List<GameObject> jackpotPrefabs = new List<GameObject>();

        public GemJackpotJackpotSymbolType symbolType = GemJackpotJackpotSymbolType.MINOR;

        private bool isSoundPlayed = false;

        private void FixedUpdate()
        {
            if (isSoundPlayed == false)
            {
                if (transform.localPosition.y < 0.0f)
                {
                    // Symbol play sound
                    GSManager.Instance.GetHandler("Meta_Gemjackpot_JackpotReel").Play();
                    isSoundPlayed = true;
                }
            }
        }

        public void Apply(BaseSymbol symbol)
        {
            if (symbolController != null)
            {
                Destroy(symbolController.gameObject);
                symbolController = null;
            }

            int symbolData = symbol.symbolInfo.symbol;
            symbolType = (GemJackpotJackpotSymbolType)symbolData;
            GameObject go = CreateSymbol(symbolData);
            if (go != null)
            {
                go.transform.SetParent(transform, false);
                symbolController = go.GetComponent<GemJackpotBonusSymbolController>();

                // Bonus symbol credit settings
                if ((GemJackpotJackpotSymbolType)symbolData == GemJackpotJackpotSymbolType.GRAND)
                    symbolController.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMA_NUMBER", NumberUtils.GetMultiplierNumeratorValue(GemJackpotUtils.GrandJackpotRewardAmount, GemJackpotUtils.EventRewardMultiply)));
                else
                {
                    List<Blackboard> jackpotReelRewardList = GemJackpotUtils.JackpotReelRewardList;
                    foreach (Blackboard bb in jackpotReelRewardList)
                    {
                        GemJackpotJackpotSymbolType type = BlackboardUtils.FindValue<GemJackpotJackpotSymbolType>(bb, "type");
                        if (type == (GemJackpotJackpotSymbolType)symbolData)
                        {
                            long credit = BlackboardUtils.FindValue<long>(bb, "credit");
                            symbolController.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMA_NUMBER", NumberUtils.GetMultiplierNumeratorValue(credit, GemJackpotUtils.EventRewardMultiply)));
                            break;
                        }
                    }
                }

                go.SetActive(true);
                // Symbol change check
                isSoundPlayed = symbol.reel.movement.spinState == SpinState.Stopped;
            }
        }

        public GameObject CreateSymbol(int symbolData)
        {
            GameObject obj = null;
            switch ((GemJackpotJackpotSymbolType)symbolData)
            {
                case GemJackpotJackpotSymbolType.MINI:
                    obj = Instantiate(jackpotPrefabs[(int)GemJackpotJackpotSymbolType.MINI]);
                    obj.name = "MINI";
                    break;
                case GemJackpotJackpotSymbolType.MINOR:
                    obj = Instantiate(jackpotPrefabs[(int)GemJackpotJackpotSymbolType.MINOR]);
                    obj.name = "MINOR";
                    break;
                case GemJackpotJackpotSymbolType.MAJOR:
                    obj = Instantiate(jackpotPrefabs[(int)GemJackpotJackpotSymbolType.MAJOR]);
                    obj.name = "MAJOR";
                    break;
                case GemJackpotJackpotSymbolType.GRAND:
                    obj = Instantiate(jackpotPrefabs[(int)GemJackpotJackpotSymbolType.GRAND]);
                    obj.name = "GRAND";
                    break;
            }
            return obj;
        }

        public void Stop(BaseSymbol symbol)
        {
            if (symbol.row == 2)
            {
                if (symbolController != null) symbolController.SetAnimationTrigger("Trigger");
            }
        }
    }
}