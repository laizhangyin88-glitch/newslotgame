using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class HogDealInGameTextController : MonoBehaviour
    {
        private ContextElement root;

        private void Start()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text", "POPUP_HOG_DEAL_IN_GAME_TAP_TO_CONTINUE", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(root, OnClick);
        }

        private void OnClick()
        {
            EventSender.SendCalleeCallback(gameObject, HogDeal.Events.ON_TAP);
            GetComponent<Animator>().SetTrigger("Close");
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
