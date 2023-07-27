using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupClubLevelUpNewController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private ContextElement membersTextElement;
        private ContextElement coCaptainsTextElement;

        private ChaseTypeLong membersChase;
        private ChaseTypeLong coCaptainsChase;

        private int clubLevel = 0;
        private int prevMembers = 0;
        private int prevCoCaptains = 0;
        private int currentMembers = 0;
        private int currentCoCaptains = 0;

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            ContextElement buttonElement = ContextUtils.FindElement(root, "Button Ok", ContextSearchingType.ChildrenSearch);
            ContextElement titleAreaElement = ContextUtils.FindElement(root, "Title Area", ContextSearchingType.ChildrenSearch);
            ContextElement membersElement = ContextUtils.FindElement(root, "Club Member Base", ContextSearchingType.ChildrenSearch);
            ContextElement coCaptainsElement = ContextUtils.FindElement(root, "Co Captain Base", ContextSearchingType.ChildrenSearch);

            membersTextElement = ContextUtils.FindElement(membersElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement membersLevelUpElement = ContextUtils.FindElement(membersElement, "Level Up", ContextSearchingType.ChildrenSearch);
            coCaptainsTextElement = ContextUtils.FindElement(coCaptainsElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement coCaptainsLevelUpElement = ContextUtils.FindElement(coCaptainsElement, "Level Up", ContextSearchingType.ChildrenSearch);

            membersChase = membersElement.GetComponent<ChaseTypeLong>();
            coCaptainsChase = coCaptainsElement.GetComponent<ChaseTypeLong>();

            // Blackboard(ClubInfo)
            Blackboard clubInfo = bb.GetValue<Blackboard>("clubInfo");
            clubLevel = clubInfo.GetValue<int>("level");
            int prevClubLevel = clubLevel - 1;
            prevMembers = ClubUtils.GetClubMaxMemberCount(prevClubLevel);
            currentMembers = ClubUtils.GetClubMaxMemberCount(clubLevel);
            prevCoCaptains = ClubUtils.GetClubMaxCoCaptainCount(prevClubLevel);
            currentCoCaptains = ClubUtils.GetClubMaxCoCaptainCount(clubLevel);

            // Text
            MetaContextElementUtils.SimpleSetTextGlobal(buttonElement, "Text", "BUTTON_OK", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(titleAreaElement, "Text Title", "POPUP_CLUB_LEVEL_UP_NEW_TITLE_0", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(titleAreaElement, "Text Bottom", "POPUP_CLUB_LEVEL_UP_NEW_TITLE_1", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Bottom", "POPUP_CLUB_LEVEL_UP_NEW_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetText(membersTextElement, prevMembers.ToString());
            MetaContextElementUtils.SetText(coCaptainsTextElement, prevCoCaptains.ToString());
            MetaContextElementUtils.SimpleSetText(root, "Text Level", clubLevel.ToString());
            MetaContextElementUtils.SimpleSetText(root, "Text Level Effect", clubLevel.ToString());

            // Click Event
            MetaContextElementUtils.SetClickable(buttonElement, ClosePopup);

            if (prevMembers == currentMembers)
                membersLevelUpElement.GetComponent<Animator>().enabled = false;
            if (prevCoCaptains == currentCoCaptains)
                coCaptainsLevelUpElement.GetComponent<Animator>().enabled = false;

            MetaSystem.SubscribeBackButton(this.GetHashCode(), ClosePopup);
            GSManager.Instance.GetHandler("UI_Club_Level_Up").Play();
            var snapshot = GSManager.Instance.GetAudioMixerSnapshot("Lobby_Popup");
            snapshot?.TransitionTo(0);
        }

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        public void OnInit()
        {
            InitProperty();
        }

        private void ClosePopup()
        {
            EventSender.SendCalleeCallback(gameObject);

            PopupManager.Instance.Close(gameObject);

            anim.SetTrigger("Close");
        }

        private void ChangeCount()
        {
            int deltaMs = 500;

            membersChase.SetNonstopChase(
                membersChase.textElement,
                (long)prevMembers,
                (long)currentMembers,
                deltaMs,
                "",
                StringTable.StringTableType.Global,
                false,
                NumberUtils.GetGlobalDenominator());

            coCaptainsChase.SetNonstopChase(
                coCaptainsChase.textElement,
                (long)prevCoCaptains,
                (long)currentCoCaptains,
                deltaMs,
                "",
                StringTable.StringTableType.Global,
                false,
                NumberUtils.GetGlobalDenominator());
        }
    }
}