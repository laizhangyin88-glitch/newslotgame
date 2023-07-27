using UnityEngine;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class PopupCouponEnterController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private ContextElement inputFrameElement;
        private ContextElement redeemButtonElement;

        private PIDButton redeemButton;

        private string inputCode = "";

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_COUPON_TITLE", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Comment", "POPUP_COUPON_TEXT", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Input Field/Placeholder", "POPUP_COUPON_PLACEHOLDER", FULL);

            redeemButtonElement = ContextUtils.FindElement(root, "Button OK", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(redeemButtonElement, "Text", "BUTTON_OK", CHILDREN);

            redeemButton = redeemButtonElement.GetComponent<PIDButton>();
            MetaContextElementUtils.SetClickable(redeemButtonElement, gameObject, "OnRedeem", false);

            inputFrameElement = ContextUtils.FindElement(root, "Input Field", CHILDREN);
            inputFrameElement.GetComponent<WebGLNativeInputField>().onValueChanged.AddListener(OnChangeInputCode);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () => EventSender.SendEvent(gameObject, "OnClose"));
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), () => EventSender.SendEvent(gameObject, "OnClose"));

            SetRedeemButtonInteractable(false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }

        public void OnRedeem()
        {
            anim.SetBool("Active", false);

            var eventData = new EventData<string>(MetaEventDefine.REDEEM_COUPON, inputCode);
            EventSender.SendGlobalMetaEvent(eventData);
        }

        public void OnIdle()
        {
            anim.SetBool("Active", true);
        }

        private void OnChangeInputCode(string code)
        {
            inputCode = code;
            bool isValid = !string.IsNullOrEmpty(code);

            SetRedeemButtonInteractable(isValid);
        }

        private void SetRedeemButtonInteractable(bool interactable)
        {
            redeemButton.interactable = interactable;
        }

        public void Close()
        {
            anim.SetTrigger("Close");
            EventSender.SendGlobalMetaEvent(MetaEventDefine.ON_LEAVE_COUPON);
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
