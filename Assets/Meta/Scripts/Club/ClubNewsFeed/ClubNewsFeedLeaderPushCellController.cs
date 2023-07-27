using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    public class ClubNewsFeedLeaderPushCellController : ClubNewsFeedLikableCellController
    {
        protected override bool IsLikable()
        {
            var myAuthority = bb.GetValue<ClubAuthority>("myAuthority");
            return myAuthority != ClubAuthority.LEADER;
        }

        protected override string GetContextText()
        {
            return StringTableUtils.GetString(GLOBAL, "CLUB_NEWS_FEED_LEADER_PUSH");
        }

        protected override string GetLikeText()
        {
            return StringTableUtils.GetString(GLOBAL, "BUTTON_LIKE_WITH_LEADER_PUSH_REWARD");
        }

        protected override IEnumerator OnLikeCoroutine()
        {
            // Check Push State
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            bool deviceNoti = NativeHelper.Instance.GetPushNotificationSubscribed();
            bool userNoti = BlackboardQueryUtils.UserOptionsPushNotification();

            // Trigger Iam
            var iamTriggerType = InAppMessageTriggerType.UNKNOWN;
            if (!deviceNoti && !userNoti)
            {
                iamTriggerType = InAppMessageTriggerType.SET_PUSH_ON_TO_GET_REWARD;
            }
            else if (!deviceNoti)
            {
                iamTriggerType = InAppMessageTriggerType.SET_PUSH_ON_AT_DEVICE_TO_GET_REWARD;
            }
            else if (!userNoti)
            {
                iamTriggerType = InAppMessageTriggerType.SET_PUSH_ON_AT_SETTINGS_TO_GET_REWARD;
            }

            if (iamTriggerType != InAppMessageTriggerType.UNKNOWN)
            {
                bool triggered = IAMRouter.Instance.TriggerIAM(iamTriggerType, gameObject, null);

                if (ApplicationSettings.LogTest())
                    Debug.Log("iamTriggerType: " + iamTriggerType + ", triggered:" + triggered);

                if (triggered)
                {
                    var iamCallbackTrigger = new EventTrigger(this, IAMUtils.ON_IAM_CALLBACK_EVENT);
                    yield return new WaitUntilTrigger(iamCallbackTrigger);

                    // Cancel Like
                    yield break;
                }
            }
#endif

            // Loading
            GameObject loadingPopupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopupObj = popupObj));

            var loadingPopupBB = loadingPopupObj.GetComponent<Blackboard>();
            loadingPopupBB.AddVariable("owner", gameObject);

            // Claim Request
            long clubId = feedInfo.GetValue<long>("id");    
            var claimReward = new Variable<Blackboard>();
            yield return StartCoroutine(ClubUtils.ClaimClubFeedCoroutine(bb, clubId, null, null, claimReward, true));

            MetaPopupUtils.ClosePopup(loadingPopupObj);

            if (claimReward != null && claimReward.value != null)
            {
                // Open Popup Mystery Gift
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Popup Mystery Gift Scene";
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                GameObject resultPopupObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                    (SceneLoadOperation sceneLoadOperation) => resultPopupObj = sceneLoadOperation.GetScene()));

                var resultPopupBB = resultPopupObj.GetComponent<Blackboard>();

                MetaObjectUtils.SetCalleeCaller(resultPopupObj, gameObject);

                var rewardList = new List<Blackboard>() { claimReward.value };
                resultPopupBB.AddVariable("_mysteryGiftList", rewardList);

                resultPopupBB.AddVariable("giftType", MysteryGiftType.LEADER_PUSH);

                MetaPopupUtils.OpenPopup(resultPopupObj);

                var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }

            OnLiked();
        }
    }
}
