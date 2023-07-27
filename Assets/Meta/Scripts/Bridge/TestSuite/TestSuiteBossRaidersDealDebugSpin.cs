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
    public class TestSuiteBossRaidersDealDebugSpin : MonoBehaviour
    {
        public GameObject scrollObj;
        public Transform contentTransform;

        public GameObject buttonPrefab;
#if DEV
        private List<DebugSpin> debugSpinList;
        private Variable<Blackboard> dealBB;

        private void Awake()
        {
            dealBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), BossRaidersUtils.BOSS_RAIDERS_DEAL_INFO);
            if (dealBB != null && dealBB.value != null)
                DrawDebugSpin();
        }

        private void DrawDebugSpin()
        {
            ClearScrollView();

            if (dealBB == null || dealBB.value == null) return;

            MakeBossRaidersDebugSpinData();

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
                        OnClickBossRaidersDealDebugSpin(debugSpin.code);
                    }
                );
                ++debugIndex;
            }
        }

        private void MakeBossRaidersDebugSpinData()
        {
            if (debugSpinList == null) debugSpinList = new List<DebugSpin>();
            if (debugSpinList.Count > 0) debugSpinList.Clear();

            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.NONE, description = BossRaidersDebugSpinType.NONE.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_DEFAULT, description = BossRaidersDebugSpinType.ATTACK_DEFAULT.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_BIG, description = BossRaidersDebugSpinType.ATTACK_BIG.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_MEGA, description = BossRaidersDebugSpinType.ATTACK_MEGA.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_EPIC, description = BossRaidersDebugSpinType.ATTACK_EPIC.ToString(), DebugSequenceList = null });
            debugSpinList.Add(new DebugSpin() { code = (int)BossRaidersDebugSpinType.BONUS, description = BossRaidersDebugSpinType.BONUS.ToString(), DebugSequenceList = null });
        }

        private void OnClickBossRaidersDealDebugSpin(int index)
        {
            if (dealBB != null && dealBB.value != null)
                BlackboardUtils.SetOrCreateValue(dealBB.value, BossRaidersUtils.BOSS_RAIDERS_DEBUG_SPIN_TYPE, (BossRaidersDebugSpinType)index);
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

        public void Close()
        {
            GameObject.Destroy(gameObject);
        }
    }
}