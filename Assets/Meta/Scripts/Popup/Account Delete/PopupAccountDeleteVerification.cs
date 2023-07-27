using UnityEngine;
using SlotMaker;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class PopupAccountDeleteVerification : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private string email;

        private string contextId = "";
        private int nthStep = 1;
        private string step = "email_enter";

        private ContextElement checkEmailElement;
        private ContextElement warningTextElement;

        private ContextElement contentsElement;
        private ContextElement emailInputTextElement;
        private ContextElement emailInputPlaceholderTextElement;
        private ContextElement submitButtonElement;

        private Animator checkEmailAnimator;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const char zeroWidthSpace = '\u200B';

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            // AE
            contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value;

            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "enter"),
                ("context_id", contextId));

            contentsElement = ContextUtils.FindElement(root, "Delete Account Verification Email Contents", CHILDREN);
            checkEmailElement = ContextUtils.FindElement(contentsElement, "Email/Check Email", FULL);
            warningTextElement = ContextUtils.FindElement(contentsElement, "Email/Text Email Warning", FULL);

            checkEmailAnimator = checkEmailElement.gameObject.GetComponent<Animator>();

            // Texts
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_VERIFICATION_TITLE", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Delete Info Text/Text", "POPUP_DELETE_ACCOUNT_VERIFICATION_INFO", FULL);

            emailInputTextElement = ContextUtils.FindElement(contentsElement, "Email/Frame Email", FULL);

            emailInputPlaceholderTextElement = ContextUtils.FindElement(contentsElement, "Email/Frame Email/Placeholder", FULL);
            MetaContextElementUtils.SetTextGlobal(emailInputPlaceholderTextElement, "POPUP_DELETE_ACCOUNT_VERIFICATION_ENTER");

            // Submit, Cancel
            submitButtonElement = ContextUtils.FindElement(root, "Button Submit", CHILDREN);
            MetaContextElementUtils.SetClickable(submitButtonElement, OnNext);
            MetaContextElementUtils.SimpleSetTextGlobal(submitButtonElement, "Text", "POPUP_DELETE_ACCOUNT_SUBMIT_BUTTON", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Cancel/Text", "POPUP_DELETE_ACCOUNT_CANCEL_BUTTON", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnCancel);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Cancel", OnCancel);

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), OnCancel);

            MetaContextElementUtils.SetListenable<string>(emailInputTextElement, OnInputChanged);

            OnInputChanged("");
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void OnInputChanged(string input)
        {
            email = input.Trim(zeroWidthSpace);

            bool isValid = RegexUtilities.IsValidEmail(email);
            bool isEmpty = string.IsNullOrEmpty(email);

            // Interactable
            submitButtonElement.GetComponent<Button>().interactable = isValid;

            // Check
            MetaContextElementUtils.SetActive(checkEmailElement, !isEmpty);
            if(checkEmailAnimator != null)
                checkEmailAnimator.SetBool("Active", isValid);

            // Warning Text
            string warningText = (isValid || isEmpty) ?
                "TEXT_BLANK" : "POPUP_DELETE_ACCOUNT_VERIFICATION_WARNING";
            MetaContextElementUtils.SetTextGlobal(warningTextElement, warningText);
        }

        private void OnCancel()
        {
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
            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "continue"),
                ("context_id", contextId));

            var eventData = new EventData<string>("OnSend", email);
            EventSender.SendCalleeCallback(gameObject, eventData);
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
