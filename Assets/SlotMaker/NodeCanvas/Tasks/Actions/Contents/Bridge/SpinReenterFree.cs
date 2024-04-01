using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SimpleJSON;
using System;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class SpinReenterFree : ActionTask
    {
        //public BBParameter<long> betCredit;
        //public BBParameter<long> extraBetCredit;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Spin Reenter Free"; } }

        protected override void OnExecute()
        {


            BeginTurn();

            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));

            BeginSpin();







            //MetaSystem.SlotSpin(betCredit.value, extraBetCredit.value, customData.value, () => { if (agent != null) EndAction(); }, null);



            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            int collectingGameChestDropRateMultiplyEventId = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId").value;
            // int collectingGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameEventID").value;
            string roomID = BlackboardUtils.GetOrCreateVariable<string>(null, "./room/roomId").value;

            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGameSpin = (spinType == SpinType.GameSpin);
            bool isBonusSpin = (spinType == SpinType.BonusSpin);
            bool isAutoSpin = BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "autoSpin").value;

            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
            // customData = ParseSlotSpinCustomData(customData);

            List<int> expEventIdList = BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY);
            expEventIdList.AddRange(BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY_EXTENDABLE));

            bool isHighRollerBet = BlackboardUtils.FindVariable<bool>("/isExtendedBetIndex")?.value ?? false;
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;


            /*if (res.HasKey("last_session_content"))
            {
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "islastFreeSpin", true);
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "lastFreeSpinContent", res["last_session_content"].ToString());
            }
            else
            {
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "islastFreeSpin", false);
            }*/

            //JSONNode json = JSONNode.Parse(globalStore.lastFreeSpinContents);


            BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "islastFreeSpin", false);

            string content = BlackboardUtils.FindVariable<string>("./lastFreeSpinContent").value;

            JSONNode json = JSONNode.Parse(content);

            long betCredit = json["data"]["contents"]["bonus_result"]["bet_credit"].AsLong;
            long extraBetCredit = 0;

            TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
            ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
            response.contents = content;  

            (new V3MetaSystem()).SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

            if (agent != null)
                EndAction();


        }



        void BeginTurn()
        {
            var cb = ContentBlackboard.Get();
            var turnVal = cb.GetVariable<Blackboard>("turn");
           if (turnVal != null)
            {
                //var history = BlackboardUtils.AddToBlackboardList(cb, "history", turnVal.value);
                //while (history.Count > historyLimit.value)
                BlackboardUtils.RemoveAtBlackboardList(cb, "history", 0);
                cb.RemoveVariable("turn");
            }
            var turn = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(cb, "turn");
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "current", turn);

            string guid = Guid.NewGuid().ToString();
            long totalBetCredit = cb.GetValue<long>("totalBetCredit");
            bool isGameSpin = cb.GetValue<bool>("isGameSpin");
            long initialCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit").value;
            long timestamp = MetaSystem.GetTimeStamp();

            BlackboardUtils.SetOrCreateValue<ContentNodeType>(turn, "type", ContentNodeType.Turn);
            BlackboardUtils.SetOrCreateValue(turn, "uid", guid);
            ContentBlackboardUtils.AddTurnCount(turn);
            BlackboardUtils.SetOrCreateValue<long>(turn, "totalBetCredit", totalBetCredit);
            ContentBlackboardUtils.AddSpentCredit(turn, isGameSpin ? 0 : totalBetCredit);
            BlackboardUtils.SetOrCreateValue<long>(turn, "earnCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(turn, "beginCredit", initialCredit);
            BlackboardUtils.SetOrCreateValue<long>(turn, "beginTime", timestamp);

            //ContentEvent.BeginTurn(turn);
            //EndAction();
        }




            protected  void BeginSpin()
            {
                var cb = ContentBlackboard.Get();
                var parent = cb.GetValue<Blackboard>("current");
                var parentType = parent.GetValue<ContentNodeType>("type");

                BlackboardUtils.FindVariable<int>(cb, "game/spinCount").value += 1;

                Blackboard spin = null;
                if (parentType == ContentNodeType.Turn)
                {
                    spin = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(parent, "spin");
                    BlackboardUtils.SetOrCreateValue<int>(spin, "spinIndex", 0);
                }
                else if (parentType == ContentNodeType.Bonus)
                {
                    spin = (Blackboard)BlackboardUtils.CreateBlackboard("spin");
                    var spinList = BlackboardUtils.AddToBlackboardList(parent, "spinList", spin);
                    BlackboardUtils.SetOrCreateValue<int>(spin, "spinIndex", spinList.Count - 1);
                }
                else
                {
                    Debug.LogError("[Content] BeginSpin failed. Spin always placed under Turn or Bonus.");
                    return;
                }
                BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "spin", spin);
                cb.SetValue("current", spin);

                string guid = Guid.NewGuid().ToString();
                long timestamp = MetaSystem.GetTimeStamp();

                BlackboardUtils.SetOrCreateValue<ContentNodeType>(spin, "type", ContentNodeType.Spin);
                BlackboardUtils.SetOrCreateValue<Blackboard>(spin, "parent", parent);
                BlackboardUtils.SetOrCreateValue(spin, "uid", guid);
                BlackboardUtils.SetOrCreateValue<long>(spin, "earnCredit", 0L);
                BlackboardUtils.SetOrCreateValue<long>(spin, "singleCredit", 0L);
                BlackboardUtils.SetOrCreateValue<long>(spin, "multiplier", 1L);
                BlackboardUtils.SetOrCreateValue<long>(spin, "beginTime", timestamp);

                //ContentEvent.BeginSpin(spin);
            }
        }















}


/*
 * 
 *             
 *             
 *             
       bool isAutoSpin = BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "autoSpin").value;
        bool autoSpin = ContentBlackboard.Get().GetValue<bool>("autoSpin");
                    BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);
            return ContentBlackboard.Get().GetVariable<bool>("autoSpin")?.value ?? false;

            var cb = ContentBlackboard.Get();
            BlackboardUtils.SetOrCreateValue(cb, "autoSpin", false);





betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            extraBetCredit = BlackboardUtils.FindVariable<long>("./extraBetCredit");
            extraBetRatioIndex = BlackboardUtils.FindVariable<int>("./extraBetRatioIndex");
            extraBetRatioList = BlackboardUtils.FindVariable<List<Blackboard>>("./game/extraBetRatioList");
            totalBetCredit = BlackboardUtils.FindVariable<long>("./totalBetCredit");
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
 * 
 */
