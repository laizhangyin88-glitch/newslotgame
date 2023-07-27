using ParadoxNotion;
using SlotMaker;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class PopupOnlinePlayersController : EventMonoBehaviour
    {
        private ContextElement root;

        private DynamicScrollOnlinePlayersCreator rect;

        private void Start()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            rect = ContextUtils.FindElement(root, "Online Players Scroll Rect",
                ContextSearchingType.ChildrenSearch).GetComponent<DynamicScrollOnlinePlayersCreator>();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCKED_USER_CHANGED, OnBlockedUserChanged);
        }

        private void OnBlockedUserChanged()
        {
            // Update List
            rect.UpdateList();
        }
    }
}
