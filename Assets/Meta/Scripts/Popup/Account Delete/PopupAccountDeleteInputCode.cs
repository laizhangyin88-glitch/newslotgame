using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace BagelCode
{
    public class PopupAccountDeleteInputCode : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private string verificationCode;
        private string email;
        private string inputCode;

        private string contextId = "";
        private int nthStep = 2;
        private string step = "verification_code_enter";

        private ContextElement contentsElement;
        private ContextElement codeInputTextElement;
        private ContextElement checkElement;
        private ContextElement deleteButtonElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

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

            email = BlackboardUtils.GetOrCreateVariable<string>(bb, "email")?.value;
            verificationCode = BlackboardUtils.GetOrCreateVariable<string>(bb, "verificationCode")?.value;

            contentsElement = ContextUtils.FindElement(root, "Delete Account Verification Email Code Contents", CHILDREN);

            codeInputTextElement = ContextUtils.FindElement(contentsElement, "Frame", CHILDREN);
            MetaContextElementUtils.SetListenable<string>(codeInputTextElement, OnInputChanged);

            checkElement = ContextUtils.FindElement(contentsElement, "Check", CHILDREN);

            // Contents
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_VERIFICATION_TITLE", FULL);

            MetaContextElementUtils.SimpleSetTextGlobal(contentsElement, "Text Top", "POPUP_DELETE_ACCOUNT_INPUT_CODE_SENT", CHILDREN, email);
            MetaContextElementUtils.SimpleSetTextGlobal(contentsElement, "Text Details", "POPUP_DELETE_ACCOUNT_INPUT_CODE_INFO", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(contentsElement, "Text Info", "POPUP_DELETE_ACCOUNT_INPUT_CODE_AGAIN", CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(codeInputTextElement, "Placeholder", "POPUP_DELETE_ACCOUNT_INPUT_CODE_ENTER", CHILDREN);

            // Delete, Cancel
            deleteButtonElement = ContextUtils.FindElement(root, "Button Delete", CHILDREN);
            MetaContextElementUtils.SetClickable(deleteButtonElement, OnDelete);
            MetaContextElementUtils.SimpleSetTextGlobal(deleteButtonElement, "Text", "POPUP_DELETE_ACCOUNT_DELETE_BUTTON", CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Cancel/Text", "POPUP_DELETE_ACCOUNT_CANCEL_BUTTON", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Cancel", OnCancel);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnCancel);

            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), OnCancel);

            OnInputChanged("");

#if DEV
            bool isSkip = PlayerPrefs.GetInt("DEBUG_SKIP_DELETE_ACCOUNT_EMAIL_CONFIRMATION", 0) == 1;
            if (isSkip)
            {
                MetaContextElementUtils.SetText(codeInputTextElement, verificationCode);
            }
#endif
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        private void OnInputChanged(string input)
        {
            inputCode = input;

            bool isValid = inputCode.Length == 5 && RegexUtilities.IsValidCode(inputCode);
            bool isEmpty = string.IsNullOrEmpty(inputCode);

            // Interactable
            deleteButtonElement.GetComponent<Button>().interactable = isValid;

            // Check
            bool showCheck = isValid || !isEmpty;
            MetaContextElementUtils.SetActive(checkElement, showCheck);
            if (showCheck)
            {
                MetaContextElementUtils.SetBooleanProperty(checkElement, isValid);
            }
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

        private void OnDelete()
        {
            if (string.Equals(inputCode, verificationCode))
            {
                OnNext();
            }
            else // Not Match
            {
                // Retry
                var popupObj = MetaPopupUtils.OpenOKPopup();
                string text = StringTableUtils.GetString(GLOBAL, "POPUP_DELETE_ACCOUNT_INPUT_CODE_FAILED");
                string buttonText = StringTableUtils.GetString(GLOBAL, "BUTTON_OK");
                MetaPopupUtils.SetCommonOKPopupData(popupObj, transform, text, buttonText);
            }
        }

        private void OnNext()
        {
            // AE
            AEUtils.SendAE("client_account_deletion_funnel",
                ("nth_step", nthStep),
                ("step", step),
                ("action_type", "continue"),
                ("context_id", contextId));

            EventSender.SendCalleeCallback(gameObject, "OnInput");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
