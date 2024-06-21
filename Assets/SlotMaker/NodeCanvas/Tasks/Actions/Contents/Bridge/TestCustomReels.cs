using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using SimpleJSON;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class TestCustomReels : ActionTask
    {
        public BBParameter<long> betCredit;
        public BBParameter<long> extraBetCredit;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Test Custom Reels Request SlotSpin"; } }

        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;

            if (TestManager.Instance.isCustomReels &&  globalStore.IsNewGame(gameId))
            {
                /*List<object> shuffling_list = new List<object>(){
                    new List<object>(){2,2,2,2,2},
                    new List<object>(){2,2,2,2,2},
                    new List<object>(){2,2,2,2,2},
                };*/

                List<object> shuffling_list = TestManager.Instance.getCustomReels();

                Dictionary<string,object> debug_param = new Dictionary<string, object>()
                {
                    {"shuffling_list", shuffling_list },
                };

                Dictionary<string, object> req = new Dictionary<string, object>
                {
                    {"bet",betCredit.value},
                    {"extra_bet",extraBetCredit.value},
                    {"debug_param", debug_param},
                };

                NetManager.Instance.Post(RPCName.new_slot_spin, req,
                (res) =>
                {
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
                    });
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                });
            }
            else
            {
                //复位假滚轮
                TestManager.Instance.ResetReel(() =>
                {
                    if (agent != null)
                        EndAction();
                });
            }

        }
    }
}
