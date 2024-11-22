using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Text.RegularExpressions;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]

    public class FriendCodeAdd : ActionTask<Blackboard>
    {
        public BBParameter<string> friendCode;

        protected override string info
        {
            get
            {
                return string.Format("Add friend by FriendCode {0}", friendCode.value);
            }
        }

        protected override void OnExecute()
        {
            // bool stringError = false;
            if (!string.Equals(friendCode.value.Trim(), string.Empty))
            {
                BagelCodeClientAPI.FriendCodeAdd(friendCode.value,
                (response) =>
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "addFriendCodeResponse");
                    bb = BlackboardUtils.GetOrCreateBlackboard(bb, "FriendInfo");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    var bbb = BlackboardUtils.GetOrCreateBlackboard(bb, "friendInfo");

                    // friend list 검사해서 이미 친구면 already
                    if (BlackboardQueryUtils.IsMyFriend(bbb.GetValue<string>("userId")))
                    {
                        BlackboardUtils.SetOrCreateValue(agent, "exceptionText", "ERROR_TARGET_ALREADY_FRIEND");
                        SendEvent("OnExceptAddFriendCode");
                    }
                    else
                    {
                        BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "friendList", bbb);

                        BlackboardQueryUtils.UpdateCelebInfo();

                        SendEvent("OnSuccessAddFriendCode");
                    }
                },
                (error) =>
                {
                    var inValid = Regex.IsMatch(friendCode.value, @"^[0-9]+$", RegexOptions.IgnoreCase);
                    if (!inValid)
                    {
                        BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_INVALID_FRIEND_CODE");
                        SendEvent("OnExceptAddFriendCode");
                    }
                    else
                    {
                        switch (error.errorCode)
                        {
                            case BagelCode.ClientModels.Error.NOT_EXIST_FRIEND_CODE_ERROR:
                                BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_NOT_EXIST_FRIEND_CODE");
                                SendEvent("OnExceptAddFriendCode");
                                break;
                            case BagelCode.ClientModels.Error.FRIEND_COUNT_MAX_ERROR:
                                BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_EXCEED_MAX_FRIEND");
                                SendEvent("OnExceptAddFriendCode");
                                break;
                            case BagelCode.ClientModels.Error.CANT_ADD_MYSELF_FRIEND_ERROR:
                                BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_CANT_ADD_MYSELF_FRIEND");
                                SendEvent("OnExceptAddFriendCode");
                                break;
                            case BagelCode.ClientModels.Error.TARGET_ALREADY_FRIEND_ERROR:
                                BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_TARGET_ALREADY_FRIEND");
                                SendEvent("OnExceptAddFriendCode");
                                break;
                            default:
                                GlobalErrorHandler.GlobalError(error);
                                SendEvent("OnFailAddFriendCode");
                                break;
                        }
                    }
                });
            }
            else
            {
                BlackboardUtils.SetOrCreateValue<string>(agent, "exceptionText", "ERROR_NEED_INPUT_FRIEND_CODE");
                SendEvent("OnExceptAddFriendCode");
            }

            EndAction();
        }
    }
}