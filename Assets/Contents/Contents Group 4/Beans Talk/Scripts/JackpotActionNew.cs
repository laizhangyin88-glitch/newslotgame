using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using SimpleJSON;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using Dreamteck.Splines.Primitives;
using SlotMaker.Slots.Tasks.Actions.Game;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class JackpotActionNew : ActionTask

    {
        //public BBParameter<long> betCredit;
        //public BBParameter<long> extraBetCredit;
        //public BBParameter<object> customData = null;

        protected override string info { get { return "Jackpot Action New"; } }

        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;

            int selectLine = BlackboardUtils.GetOrCreateVariable<int>(null, "./gameNew/selectLine").value;
            long betCredit = BlackboardUtils.GetOrCreateVariable<long>(null, "./totalBetCredit").value;

            Dictionary<string, object> debug_param = new Dictionary<string, object>()
                {
                    {"jackpot_id", 3 },
                };

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                    {"bet",betCredit/selectLine},
                    //{"extra_bet",0 },
                    {"win_line_count",selectLine},
                    {"debug_param", debug_param},
            };

            NetManager.Instance.Post(RPCName.newSlotSpin, req,
            (res) =>
            {


                string resStr = res.ToString();
               // var bb = ContentBlackboard.Get();
               // var gameNew = BlackboardUtils.GetOrCreateBlackboard(bb, "gameNew");
               // BlackboardUtils.SetOrCreateValue(gameNew, "responseJackpot", resStr);

                Blackboard bonusBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "bonus");
                BlackboardUtils.SetOrCreateValue(bonusBB, "responseNew", resStr);
                BlackboardUtils.SetOrCreateValue(bonusBB, "bonusName", "jackpot");


                /*
                // 创建假的滚轮
                TestManager.Instance.ChangeReel(shuffling_list,
                (List<int> reelsIdex) =>
                {
                    res["game_result"]["first_index_list"] = JSONNode.Parse("[]");

                    foreach (var idx in reelsIdex)
                    {
                        res["game_result"]["first_index_list"].Add(idx);
                    }

                    TestManager.Instance.customReelsSpinRes = res.ToString();

                    Debug.Log($"==@ 自定义滚轮数据 {TestManager.Instance.customReelsSpinRes}");
                    if (agent != null)
                        EndAction();
                });*/

                if (agent != null)
                    EndAction();
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });

        }
    }
}
