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
    public class TestSuiteMetaDebugSpin : MonoBehaviour
    {
        public GameObject scrollObj;
        public Transform contentTransform;

        public GameObject buttonPrefab;

        private EventInfo eventInfo;

        private int debugSpinType = 0;

#if DEV
        private List<DebugSpin> debugSpinList;

        private void Awake()
        {
            eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            if (eventInfo != null)
                debugSpinList = BlackboardQueryUtils.GetMetaGameDebugSpin(eventInfo.type);

            if (debugSpinList != null && debugSpinList.Count > 0)
                DrawMetaDebugSpin();
        }

        private void DrawMetaDebugSpin()
        {
            ClearScrollView();
            if (debugSpinList == null || debugSpinList.Count == 0) return;

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
                        OnClickMetaDebugSpin(debugSpin.code);
                    }
                );
                ++debugIndex;
            }

        }

        private void OnClickMetaDebugSpin(int index)
        {
            BlackboardQueryUtils.SetMetaGameDebugSpin(eventInfo.type, index);
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