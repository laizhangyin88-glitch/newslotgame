using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbySlotControllerJackpotBoard : MonoBehaviour
    {
        public List<GameObject> jackpotBoardList;
        public List<GameObject> jackpotBoardTextList;
        private ContextElement selfElement;

        private Blackboard  slotInfoBB;
        private Blackboard  jackpotInfo;
        private List<Blackboard> jackpotList;

        private ContextElement jackpotIconAreaElement;

        private GameObject jackpotIconObject;

        private bool isInit = false;

        private void InitContext()
        {
            if(isInit) return;

            selfElement = gameObject.GetComponent<ContextElement>();
            selfElement.UpdateContext(false);

            jackpotIconAreaElement = ContextUtils.FindElement(selfElement, "Jackpot Icon Area", ContextSearchingType.ChildrenSearch);

            isInit = true;

        }

        public void SetJackpot(Blackboard slotInfoBB, Blackboard jackpotInfoBB, List<Blackboard> jackpotList)
        {
            InitContext();

            if(jackpotIconObject != null)
                GameObject.Destroy(jackpotIconObject);

            int makeCount = 0;

            if(jackpotInfoBB != null)
            {
                var jackpotAssetType = BlackboardUtils.FindVariable<JackpotAssetType>(jackpotInfoBB, "jackpotAssetType");
                jackpotIconObject = MetaIconUtils.MakeJackpotIconObject(jackpotAssetType.value, jackpotIconAreaElement.transform, null);
                ++makeCount;
            }

            for(int i=0; i < jackpotBoardList.Count; ++i)
            {
                jackpotBoardList[i].SetActive(false);
            }

            int viewLimitCount = 1;
            var isViewLong = slotInfoBB.GetVariable<bool>("isViewLong");
            if(isViewLong != null && isViewLong.value)
                viewLimitCount = 4;

            for(int i=0; i < jackpotList.Count; ++i)
            {
                if(viewLimitCount <= makeCount) break;
                ++makeCount;

                BlackboardQueryUtils.InitJackpotInfo(jackpotList[i]);

                // var element = ContextUtils.FindElement(agent, elementName.value, searchingType);
                int jackpotChaseIndex = jackpotList.Count - i - 1;  //값이 큰 것부터 넣어준다.(역순)
                var jackpotChase = jackpotBoardTextList[jackpotChaseIndex].GetComponent<ChaseTypeLong>();
                jackpotChase.SetNonstopChase
                (
                    jackpotChase.textElement,
                    jackpotList[i].GetValue<long>("prev"),
                    jackpotList[i].GetValue<long>("current"),
                    jackpotList[i].GetValue<int>("deltaMs"),
                    null,
                    StringTable.StringTableType.Global,
                    true,
                    NumberUtils.GetGlobalDenominator(),
                    jackpotList[i].GetVariable<long>("progress")
                );

                jackpotBoardList[i].SetActive(true);
            }

            jackpotIconAreaElement.gameObject.SetActive(jackpotInfoBB != null);
        }


    }
}
