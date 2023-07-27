using System.Collections;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.BI;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using static BagelCode.InboxEvent;

namespace BagelCode
{
    public class InboxBannerController : MonoSingleton<InboxBannerController>
    {
        // Todo DoAction 다른 곳에서도 쓰임
        // InboxCellData 처럼 ActionData 만들어서 처리
        // 일단 보류
        public InboxController ownerInboxController;

        private ContextElement root;
        private Blackboard rootBB;

        private Blackboard noticeInfo;
        private Blackboard actionInfo;

        private string contextId = string.Empty;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        //

        private void Start()
        {
            InitProperty();
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();

            noticeInfo = rootBB.GetValue<Blackboard>("noticeInfo");

            root.UpdateContext(true);

            StartCoroutine(ImageLoadCoroutine());
        }

        private IEnumerator ImageLoadCoroutine()
        {
            string imageUrl = BlackboardUtils.GetOrCreateVariable<string>("noticeInfo/imageUrl").value;

            bool isSuccess = false;
            bool isFailure = false;
            MetaContextElementUtils.SimpleSetWebImage(root, "Button Banner/Banner", imageUrl, () => isSuccess = true, () => isFailure = true, FULL);

            yield return new WaitUntil(() => isSuccess || isFailure);

            if(isFailure)
            {
                Destroy(gameObject);
                yield break;
            }

            BiEventUtils.SendBiClientNotice(rootBB, false, "noticeInfo", 0, ref contextId);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Banner",
                () => EventSender.SendEvent(gameObject, ON_CLICK_BANNER), false, CHILDREN);

            var actionInfoVar = noticeInfo.GetVariable<Blackboard>("action");

            if (actionInfoVar != null)
                actionInfo = actionInfoVar.value;

            // InitEvents(); todo shk
        }

        //private void InitEvents() todo shk
        //{
        //    // Click Banner
        //    var onClickBannerTrigger = new EventTrigger(this, ON_CLICK_BANNER);
        //    RegisterHandlingEvent(onClickBannerTrigger, DoActionCoroutine);

        //    // Connect
        //    var onConnectTrigger = new EventTrigger(this, ON_CONNECT_COMPLETE);
        //    RegisterHandlingEvent(onConnectTrigger, () => ownerInboxController.Dispatch(ON_CONNECT_COMPLETE));
        //}

        private IEnumerator DoActionCoroutine()
        {
            BiEventUtils.SendBiClientNotice(rootBB, true, "noticeInfo", 0, ref contextId);

            var type = actionInfo.GetVariable<ActionType>("type")?.value ?? ActionType.NONE;

            switch(type)
            {
                case ActionType.FACEBOOK_INVITE:
                    {
                        Debug.Log("Invite State");
                        string requestTitle = BlackboardUtils.FindValue<string>("/values/misc/FACEBOOK_GAMEREQUEST_TITLE");
                        string requestMessage = BlackboardUtils.FindValue<string>("/values/misc/FACEBOOK_GAMEREQUEST_MESSAGE");
#if !UNITY_EDITOR
                        SocialManager.Instance.InviteFB(requestMessage, requestTitle,
                            (string json) =>
                            {
                                // EndAction();
                                // if (json.Equals("Error"))
                                // {
                                //     SendEvent("OnInviteError");
                                // }
                                // else 
                                // {
                                //     SendEvent("OnInviteSuccess");
                                // }
                            });
#else
                        EventSender.SendEvent(gameObject, ON_INVITE_ERROR);
#endif
                        EventSender.SendEvent(gameObject, ON_SUCCESS_FACEBOOK_INVITE);
                    }
                    break;
                case ActionType.FACEBOOK_CONNECT:
                    {
                        PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int)BI_client_vip_club_funnel.BIVIPClubFunnelType.FB_CONNECT_ACTION);
                        PlayerPrefs.SetString("VIP_CLUB_FUNNEL_CONTEXT_ID", BiEventUtils.GenerateContextID());
                    }
                    break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
