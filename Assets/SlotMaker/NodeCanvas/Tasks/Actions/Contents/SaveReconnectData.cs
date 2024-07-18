using BagelCode;
using NodeCanvas.Framework;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class SaveReconnectData : ActionTask
    {
        protected override void OnExecute()
        {
            var bb = ContentBlackboard.Get();
            BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "spin", new Blackboard());
            var spin = bb.GetVariable<Blackboard>("spin").value;
            var gameNew = BlackboardUtils.GetOrCreateBlackboard(bb, "gameNew");
            var temp = BlackboardUtils.GetOrCreateVariable<JSONNode>(gameNew, "ConnectData", true);
            if (temp != null && temp.value != null)
            {
                var gameId = BlackboardUtils.FindValue<int>("./game/gameId");
                SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
                V3MetaSystem.SlotSpinSuccessNew(temp.value, gameId, 0, 0, spinType);

                int currentCount = temp.value["free_game_result"]["cur_free_game_times"].AsInt;
                int totalCount = temp.value["free_game_result"]["max_free_game_times"].AsInt;

                var bonus = (Blackboard)BlackboardUtils.CreateBlackboard("bonus");
                BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "bonus", bonus);
                bb.SetValue("current", bonus);
                var parent = bb.GetValue<Blackboard>("current");
                BlackboardUtils.SetOrCreateValue<int>(bonus, "initialSpinCount", totalCount);
                if(currentCount <= 15)
                {
                    BlackboardUtils.SetOrCreateValue<int>(bonus, "spinCount", currentCount - 1);
                }
                else
                {
                    BlackboardUtils.SetOrCreateValue<int>(bonus, "spinCount", currentCount);
                }
                BlackboardUtils.SetOrCreateValue<int>(bonus, "totalSpinCount", totalCount);
                BlackboardUtils.SetOrCreateValue<long>(bonus, "beginTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                BlackboardUtils.SetOrCreateValue<ContentNodeType>(bonus, "type", ContentNodeType.Bonus);
                //BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "parent", parent); 
                 
                string guid = Guid.NewGuid().ToString();
                BlackboardUtils.SetOrCreateValue(bonus, "uid", guid);
                BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusId", globalStore.bonusID[gameId]);
                BlackboardUtils.SetOrCreateValue<long>(bonus, "multiplier", 1L);

                int selectLine = temp.value["client_data"]["win_line_count"].AsInt;
                BlackboardUtils.SetOrCreateValue(gameNew, "selectLine", selectLine);
                long bet = temp.value["client_data"]["bet"].AsLong;
                BlackboardUtils.GetOrCreateVariable<long>("./betCredit").value = bet * selectLine;
                long extraBetCredit = temp.value["client_data"]["extra_bet"].AsLong;
                BlackboardUtils.SetOrCreateValue(bb, "extraBetCredit", extraBetCredit);

                //BlackboardUtils.GetOrCreateVariable<long>(null, "./turn/totalBetCredit").value = BlackboardUtils.FindVariable<long>("./totalBetCredit").value;
                var turn = (Blackboard)BlackboardUtils.CreateBlackboard("turn");
                BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "turn", turn);
                var meCredit = BlackboardUtils.FindVariable<long>("/me/credit");
                BlackboardUtils.SetOrCreateValue<long>(turn, "totalBetCredit", meCredit.value);

                var spinBB = (Blackboard)BlackboardUtils.CreateBlackboard("spin");
                BlackboardUtils.SetOrCreateValue<Blackboard>(turn, "spin", spinBB);
                var response = (Blackboard)BlackboardUtils.CreateBlackboard("respones");
                BlackboardUtils.SetOrCreateValue<Blackboard>(spinBB, "response", response); 
                var userSyncInfo = (Blackboard)BlackboardUtils.CreateBlackboard("userSyncInfo");
                BlackboardUtils.SetOrCreateValue<Blackboard>(response, "userSyncInfo", userSyncInfo);
                BlackboardUtils.SetOrCreateValue<int>(userSyncInfo, "totalSpinCount", 1);
            }
            EndAction();
        }
    }
}
