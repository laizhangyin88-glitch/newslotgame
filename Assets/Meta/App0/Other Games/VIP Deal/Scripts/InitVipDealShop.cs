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
    public class InitVipDealShop : ActionTask<Blackboard>
    {
        public BBParameter<string> contextID;

        public BBParameter<long> saveAsEndTimestamp;
        public BBParameter<long> saveAsWarningTimestamp;

        public List<Blackboard> sortedDealList;

        protected override string info
        {
            get { return "Init Vip Deal Shop UI"; }
        }

        protected override void OnExecute()
        {
            var agentElement = agent.gameObject.GetComponent<ContextElement>();

            var vipDealInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "vipDealInfo");

            var dealList = vipDealInfo.value.GetValue<List<Blackboard>>("dealList");
            sortedDealList = new List<Blackboard>();
            sortedDealList.AddRange(dealList);
            sortedDealList.Sort(
                        delegate (Blackboard source, Blackboard dest)
                        {
                            var sourceFree = source.GetValue<bool>("isFree");
                            if(sourceFree) return -1;
                            var destFree = dest.GetValue<bool>("isFree");
                            if(destFree) return 1;

                            var sourceMultiplierNumerator = source.GetValue<long>("multiplierNumerator");
                            var destMultiplierNumerator = dest.GetValue<long>("multiplierNumerator");

                            return destMultiplierNumerator.CompareTo(sourceMultiplierNumerator);
                        }
                    );

            var itemCell01 = ContextUtils.FindElement(agentElement, "01/VIP Deal Shop Item", ContextSearchingType.FullNameSearch);
            var itemCell02 = ContextUtils.FindElement(agentElement, "02/VIP Deal Shop Item", ContextSearchingType.FullNameSearch);
            var itemCell03 = ContextUtils.FindElement(agentElement, "03/VIP Deal Shop Item", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetBlackboardValue<Blackboard>(itemCell01, "dealInfo", sortedDealList[1]);
            MetaContextElementUtils.SetBlackboardValue<Blackboard>(itemCell02, "dealInfo", sortedDealList[0]);
            MetaContextElementUtils.SetBlackboardValue<Blackboard>(itemCell03, "dealInfo", sortedDealList[2]);

            MetaContextElementUtils.SetBlackboardValue<string>(itemCell01, "_biContextID", contextID.value);
            MetaContextElementUtils.SetBlackboardValue<string>(itemCell02, "_biContextID", contextID.value);
            MetaContextElementUtils.SetBlackboardValue<string>(itemCell03, "_biContextID", contextID.value);

            var vipDealInfoID = vipDealInfo.value.GetValue<int>("vipDealInfoId");
            MetaContextElementUtils.SetBlackboardValue<int>(itemCell01, "_vipDealInfoID", vipDealInfoID);
            MetaContextElementUtils.SetBlackboardValue<int>(itemCell02, "_vipDealInfoID", vipDealInfoID);
            MetaContextElementUtils.SetBlackboardValue<int>(itemCell03, "_vipDealInfoID", vipDealInfoID);

            var endTimestamp = vipDealInfo.value.GetValue<long>("endTimestamp");

            saveAsEndTimestamp.value = endTimestamp;
            saveAsWarningTimestamp.value = endTimestamp - 3600000;

            EndAction();
        }
    }
}
