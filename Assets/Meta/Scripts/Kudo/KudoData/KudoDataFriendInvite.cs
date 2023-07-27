using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataFriendInvite : KudoData
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Friend Invite Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            // Set Button Text
            SetText(0, "BUTTON_ACCEPT");
            SetButtonTextActive(1, false);
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            // Send Bi
            string targetUserId = info.GetValue<string>("userId");
            string kudoType = "kudo_invite";
            SendBiKudo("trigger", targetUserId, 0, kudoType);

            // Check Game, Slot Info
            int gameId = info.GetValue<int>("gameId");
            var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
            var slotInfo = BlackboardQueryUtils.GetSlotInfoBB(gameId, out bool isEarlyAccess);

            if (gameInfo == null || slotInfo == null) yield break;

            int slotState = BlackboardUtils.FindValue<int>(slotInfo, "flags/status");
            var unlockState = gameInfo.GetValue<GameUnlockStatus>("unlockStatus");

            int minLevel = gameInfo.GetValue<int>("minLevel");
            var level = BlackboardUtils.FindVariable<int>("/me/level")?.value ?? 0;

            if (slotState == 0 &&
                unlockState != GameUnlockStatus.LOCKED &&
                level >= minLevel)
            {
                bool isInGame = BlackboardUtils.FindVariable<bool>("/inGame")?.value ?? false;
                if (isInGame)
                {
                    // Check Room ID
                    string roomId = info.GetValue<string>("roomId");
                    string contentRoomId = BlackboardUtils.FindVariable<string>("./room/roomId")?.value;

                    if (roomId != contentRoomId)
                    {
                        // Active Anim
                        ActiveAnimator();

                        // Show Profile
                        yield return controller.StartCoroutine(SetProfileCoroutine(0, info));

                        // Set Center Text
                        string name = info.GetValue<string>("name");
                        SetCenterTextElement("FEED_INVITE_TEXT", name, gameId);

                        // Make Slot Thumbnail Icon
                        MakeSlotThumbnailIcon(gameInfo, false);

                        // Accept | Wait | Skip
                        var onAcceptTrigger = new EventTrigger(controller, ON_ACCEPT);
                        var timerTrigger = new TimerTrigger(KUDO_DISPLAY_TIME);
                        var onSkipTrigger = new EventTrigger(controller, ON_SKIP);
                        yield return new WaitUntilTrigger(onAcceptTrigger, timerTrigger, onSkipTrigger);

                        // On Accept
                        if (onAcceptTrigger.IsTrigger)
                        {
                            // Send Bi Click
                            SendBiKudo("click", targetUserId, 0, kudoType);

                            bool isEarlyAccessAvailable = BlackboardQueryUtils.IsEarlyAccessAvailable();
                            if (isEarlyAccess && !isEarlyAccessAvailable)
                            {
                                // Trigger IAM
                                IAMRouter.Instance.TriggerIAM(
                                    InAppMessageTriggerType.EARLY_ACCESS_NON_SUBSCRIBER_SLOT_ENTER, null, "");
                            }
                            else
                            {
                                // Set EarlyAccess Game Info
                                bool toEarlyAccess = isEarlyAccess && isEarlyAccessAvailable;
                                BlackboardQueryUtils.SetEnterGameInfo(
                                    gameId, "EnterGame", "invite",
                                    roomId, 0, null, toEarlyAccess);

                                EventSender.SendGlobalEvent("OnEnterGame");
                            }
                        }

                        // Disappear
                        yield return controller.StartCoroutine(DisappearCoroutine());
                    }
                }
                else // Not InGame
                {
                    // Make Invited Popup
                    string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                    string asset = "Popup Invited at lobby Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    GameObject popupObj = null;
                    yield return controller.StartCoroutine(
                        MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                        (SceneLoadOperation operation) => popupObj = operation.GetScene()));

                    var popupBB = popupObj.GetComponent<Blackboard>();
                    popupBB.AddVariable("inviteInfo", info);

                    MetaObjectUtils.SetCalleeCaller(popupObj, controller.gameObject);
                    MetaPopupUtils.OpenPopup(popupObj);

                    // Until Popup Closed
                    yield return new WaitUntilTrigger( new WaitUntilConditionTrigger(() => popupObj == null));

                    SendBiKudo("click", targetUserId, 0, kudoType);
                }
            }
        }
    }
}
