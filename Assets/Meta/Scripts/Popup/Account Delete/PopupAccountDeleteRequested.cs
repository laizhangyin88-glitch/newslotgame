using SlotMaker;

namespace BagelCode
{
    public class PopupAccountDeleteRequested : EventMonoBehaviour
    {
        private ContextElement root;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            // Texts
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_REQUESTED_TITLE", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Delete Account Removal Requested Text", "POPUP_DELETE_ACCOUNT_REQUESTED_INFO", CHILDREN);
        }
    }
}
