using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Vip Deal")]
    public class InitVipDealSelect : ActionTask<Blackboard>
    {
        public BBParameter<List<GameObject>> saveAsItemCellList;
        public BBParameter<List<Blackboard>> saveAsSelectItemList;
        public BBParameter<int> saveAsFreeProductIndex;

        private const int ITEM_CELL_COUNT = 6;
        private ContextElement agentElement;
        private List<long> multiplierNumeratorList;

        protected override string info
        {
            get { return "Init Vip Deal UI ( Wheels, DealInfo, Cell )"; }
        }

        protected override void OnExecute()
        {
            agentElement = agent.gameObject.GetComponent<ContextElement>();

            saveAsFreeProductIndex.value = -1;
            multiplierNumeratorList = BlackboardUtils.FindVariable<List<long>>(MainBlackboard.Get(), "vipDealInfo/multiplierNumeratorList")?.value ?? new List<long>();

            InitCellList();
            InitWheelMultiplier();
            InitDealInfo();

            EndAction();
        }

        private void InitCellList()
        {
            saveAsItemCellList.value = new List<GameObject>();

            for (int i = 0; i < ITEM_CELL_COUNT; ++i)
            {
                var itemCell = ContextUtils.FindElement(agentElement, string.Format("VIP Deal Item Area/Item {0:00}", i + 1), ContextSearchingType.FullNameSearch);

                MetaContextElementUtils.SetBlackboardValue<int>(itemCell, "index", i);

                saveAsItemCellList.value.Add(itemCell.gameObject);
            }
        }

        private void InitWheelMultiplier()
        {
            for (int i = 0; i < multiplierNumeratorList.Count; ++i)
            {
                var textElement = ContextUtils.FindElement(agentElement, string.Format("Wheel/Base/{0:00}/Text", i + 1), ContextSearchingType.FullNameSearch);
                double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumeratorList[i]);
                MetaContextElementUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_WHEEL_MULTIPLIER_TEXT", multiplier));
            }
        }

        private void InitDealInfo()
        {
            saveAsSelectItemList.value = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "vipDealInfo/dealList").value;

            for (int i = 0; i < saveAsSelectItemList.value.Count; ++i)
            {
                long dealMultiplierNumermator = saveAsSelectItemList.value[i].GetValue<long>("multiplierNumerator");
                int index = multiplierNumeratorList.IndexOf(dealMultiplierNumermator);

                BlackboardUtils.SetOrCreateValue<int>(saveAsSelectItemList.value[i], "wheelIndex", index);
                BlackboardUtils.SetOrCreateValue<float>(saveAsSelectItemList.value[i], "wheelAngle", (float)index * 30f);

                bool isFree = saveAsSelectItemList.value[i].GetValue<bool>("isFree");
                if (isFree)
                    saveAsFreeProductIndex.value = i;
            }
        }
    }
}