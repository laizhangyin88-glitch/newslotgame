using BagelCode;
using NodeCanvas.Framework;
using SlotMaker.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace SlotMaker.Tasks.Actions.Contents
{
    public class SendNewSlotSpin : ActionTask
    {
        protected override void OnExecute()
        {
            Debug.LogError("JJJJJJJJJJJJJJJJJJJJJJJJJ jackpot send message......................");
            int selectLine = BlackboardUtils.GetOrCreateVariable<int>(null, "./gameNew/selectLine").value;
            long betCredit = BlackboardUtils.FindVariable<long>("./betCredit").value;
            long extraBetCredit = BlackboardUtils.FindVariable<long>("./extraBetCredit").value;

            int jackpot = int.Parse(TestManager.Instance.getJackpot());

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"bet",betCredit/selectLine},
                {"extra_bet",extraBetCredit },
                {"win_line_count",selectLine},
            };
            if (jackpot > 0)
            {
                Dictionary<string, object> debug_param = new Dictionary<string, object>
                {
                    {"jackpot_id", jackpot}
                };
                req.Add("debug_param", debug_param);
            }
            NetManager.Instance.Post(RPCName.newSlotSpin, req, (res) =>
            {
                Debug.LogError("res.............................." + res.ToString());
                 
                var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
                if (spinBB == null)
                {
                    spinBB = new Variable<Blackboard>();
                    spinBB.value = new Blackboard();
                }
                BlackboardUtils.SetOrCreateValue(spinBB.value, "responseNew", res.ToString());
            },
            (error) =>
            {
                CommonError(error);
            });
            EndAction();
        }

        private void CommonError(BagelCodeHTTPError error)
        {
            Debug.LogWarning("V3MetaSystem.CommonError invoked!!");
            Debug.LogError(SlotSimpleJson.SerializeObject(error));

            switch (error.errorCode)
            {
                case BagelCode.ClientModels.Error.NOT_IN_ROOM_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();
                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_NOT_EXIST_ROOM", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                        info.callback1 = delegate
                        {
                            MessageDispatcher.Dispatch("OnContentEvent", new EventData("LeaveGame"));
                        };

                        ErrorPopupHandler.Instance.OpenError(info);
                    }
                    break;

                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        }
    }
}
