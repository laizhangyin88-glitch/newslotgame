using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class PopupAccountDeleteConfirm : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private bool isInfo = false;

        private string contextId = "";
        private int nthStep = 0;
        private string step = "";

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // Texts
            isInfo = BlackboardUtils.GetOrCreateVariable<bool>(bb, "isInfo")?.value ?? true;

            if (isInfo)
            {
                nthStep = 0;
                step = "delete_information";
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_INFO_TITLE", FULL);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Delete Info Text/Text", "POPUP_DELETE_ACCOUNT_INFO_INFO", FULL);
            }
            else
            {
                nthStep = 3;
                step = "final_confirmation";
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_CONFIRM_TITLE", FULL);
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Delete Info Text/Text", "POPUP_DELETE_ACCOUNT_CONFIRM_INFO", FULL);
            }

            // Buttons
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Delete/Text", "POPUP_DELETE_ACCOUNT_DELETE_BUTTON", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Cancel/Text", "POPUP_DELETE_ACCOUNT_CANCEL_BUTTON", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Delete", OnNext);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Cancel", OnCancel);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnCancel);

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), OnCancel);

            // AE
            contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value;

            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "enter"),
                ("context_id", contextId));
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void OnCancel()
        {
            // AE
            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "cancel"),
                ("context_id", contextId));

            EventSender.SendCalleeCallback(gameObject, MetaEventDefine.ON_CANCEL);
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void OnNext()
        {
            // AE
            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "continue"),
                ("context_id", contextId));

            EventSender.SendCalleeCallback(gameObject, isInfo ? "OnNext" : "OnConfirm");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
