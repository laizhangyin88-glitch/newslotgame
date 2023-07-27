using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using TMPro;
using SlotMaker.TestSuite;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class TestSuiteVipLoungeJackpotDebugSpin : MonoBehaviour
    {
        public GameObject scrollObj;
        public Transform contentTransform;

        public GameObject buttonPrefab;

#if DEV
        private List<DebugSpin> debugSpinList;

        private void Awake()
        {
            DrawDebugSpin();
        }

        private void DrawDebugSpin()
        {
            ClearScrollView();
            debugSpinList = new List<DebugSpin>();

            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.UNKNOWN, description = LoungeJackpotWinType.UNKNOWN.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.CREDIT, description = LoungeJackpotWinType.CREDIT.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.MINI, description = LoungeJackpotWinType.MINI.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.MINOR, description = LoungeJackpotWinType.MINOR.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.MAJOR, description = LoungeJackpotWinType.MAJOR.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)LoungeJackpotWinType.GRAND, description = LoungeJackpotWinType.GRAND.ToString(), DebugSequenceList = null });

            int debugIndex = 1;
            foreach (var debugSpin in debugSpinList)
            {
                GameObject go = GameObject.Instantiate(buttonPrefab) as GameObject;
                go.transform.SetParent(contentTransform, false);

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = debugIndex.ToString();
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = debugSpin.description;
                go.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        OnClickDebugSpin(debugSpin.code);
                    }
                );
                ++debugIndex;
            }
        }

        private void OnClickDebugSpin(int index)
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), VipLounge.VipLounge.Defines.LOUNGE_JACKPOT_DEBUG_SPIN, (LoungeJackpotWinType)index);
            Close();
        }

        private void ClearScrollView()
        {
            int childs = contentTransform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(contentTransform.GetChild(i).gameObject);
            }
        }
#endif
        public void SetGameSpeed(float gameSpeed)
        {
            Time.timeScale = gameSpeed;
        }

        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}