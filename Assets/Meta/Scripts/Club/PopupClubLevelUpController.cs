using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupClubLevelUpController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private int clubLevel;

        private void Start()
        {
            InitProperty();
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            var clubInfo = bb.GetValue<Blackboard>("clubInfo");
            clubLevel = clubInfo.GetValue<int>("level");

            MetaContextElementUtils.SimpleSetTextGlobal(
                root, "Title Area/Text", "POPUP_CLUB_LEVEL_UP_TITLE", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SimpleSetTextGlobal(
                root, "Text", "POPUP_CLUB_LEVEL_UP_TEXT", ContextSearchingType.ChildrenSearch, clubLevel);

            MetaContextElementUtils.SimpleSetClickable(
                root, "Button Ok", ClosePopup);

            MetaSystem.SubscribeBackButton(this.GetHashCode(), ClosePopup);
        }

        private void ClosePopup()
        {
            EventSender.SendCalleeCallback(gameObject);

            PopupManager.Instance.Close(gameObject);

            anim.SetTrigger("Close");
        }
    }
}