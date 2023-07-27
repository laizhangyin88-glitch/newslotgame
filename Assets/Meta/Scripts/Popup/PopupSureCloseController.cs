using SlotMaker;

namespace BagelCode
{
    public class PopupSureCloseController : EventMonoBehaviour
    {
        private ContextElement root;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            MetaContextElementUtils.SimpleSetClickable(root, "Button NoThanks", Close);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button NoThanks/Text", "POPUP_IAM_SURE_CLOSE_BUTTON_CLOSE", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button ShowDeal", Return);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button ShowDeal/Text", "POPUP_IAM_SURE_CLOSE_BUTTON_RETURN", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", Return);
        }

        private void Return()
        {
            EventSender.SendCalleeCallback(gameObject, "OnReturn");
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void Close()
        {
            EventSender.SendCalleeCallback(gameObject, "OnClose");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
