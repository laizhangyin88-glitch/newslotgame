using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class PopupPreviewBannerAreaController : MonoBehaviour
    {
        private ContextCompositor root;

        private void Start()
        {
            root = GetComponent<ContextCompositor>();
            root.UpdateContext();

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", OnClose);
        }

        private void OnClose()
        {
            EventSender.SendCalleeCallback(gameObject);
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
