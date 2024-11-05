using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Utils")]
    public class InitAnnouncementSetting : ActionTask<ContextElement>
    {
        public enum SettingType
        {
            GLOBAL_CHAT,
            KUDO_JACKPOT,
            KUDO_TORNAMENT,
            KUDO_NEW_USER_WELCOME,
            PIP_MODE,
        }

        private enum ButtonType
        {
            ON,
            OFF,
            BASE,
        }

        public BBParameter<SettingType> typeInfo;

        #region Properties

        public static string GetCellName(SettingType type)
        {
            return typeCellDict[type];
        }

        public static string GetKey(SettingType type)
        {
            return typeKeyDict[type];
        }

        public static string GetEventName(SettingType type)
        {
            return eventNameDict[type];
        }

        public static string GetTitleTextKey(SettingType type)
        {
            return titleTextKeyDict[type];
        }

        private string CellName
        {
            get => GetCellName(typeInfo.value);
        }

        private string Key
        {
            get => GetKey(typeInfo.value);
        }

        private string EventName
        {
            get => GetEventName(typeInfo.value);
        }

        private string TitleTextKey
        {
            get => GetTitleTextKey(typeInfo.value);
        }

        #endregion

        private bool state;

        private static bool isInitDict = false;

        private static Dictionary<SettingType, string> typeCellDict = new Dictionary<SettingType, string>();
        private static Dictionary<SettingType, string> typeKeyDict = new Dictionary<SettingType, string>();
        private static Dictionary<SettingType, string> eventNameDict = new Dictionary<SettingType, string>();
        private static Dictionary<SettingType, string> titleTextKeyDict = new Dictionary<SettingType, string>();

        protected override string info
        {
            get
            {
                return string.Format("Init {0}", typeInfo.value);
            }
        }

        protected override void OnExecute()
        {
            SetDict();

            agent.UpdateContext();

            ContextElement cellElement = ContextUtils.FindElement(agent, CellName, ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetActive(cellElement, true);

            state = BlackboardUtils.GetOrCreateVariable<bool>(Key).value;

            SetTitle();
            SetButton();

            EndAction(true);
        }

        private void SetDict()
        {
            if (isInitDict) return;

            typeCellDict.Add(SettingType.GLOBAL_CHAT, "Global Chat Cell");
            typeCellDict.Add(SettingType.KUDO_JACKPOT, "Jackpot Winner Cell");
            typeCellDict.Add(SettingType.KUDO_TORNAMENT, "Tournament Winner Cell");
            typeCellDict.Add(SettingType.KUDO_NEW_USER_WELCOME, "New User Alarm Cell");

            typeKeyDict.Add(SettingType.GLOBAL_CHAT, "/userOptions/globalChat");
            typeKeyDict.Add(SettingType.KUDO_JACKPOT, "/userOptions/kudoJackpot");
            typeKeyDict.Add(SettingType.KUDO_TORNAMENT, "/userOptions/kudoTournament");
            typeKeyDict.Add(SettingType.KUDO_NEW_USER_WELCOME, "/userOptions/kudoNewUserWelcome");
            typeKeyDict.Add(SettingType.PIP_MODE, "/userOptions/enablePipMode");

            eventNameDict.Add(SettingType.GLOBAL_CHAT, "OnSettingGlobalChat");
            eventNameDict.Add(SettingType.KUDO_JACKPOT, "OnSettingJackpot");
            eventNameDict.Add(SettingType.KUDO_TORNAMENT, "OnSettingTournament");
            eventNameDict.Add(SettingType.KUDO_NEW_USER_WELCOME, "OnSettingNewUser");

            titleTextKeyDict.Add(SettingType.GLOBAL_CHAT, "POPUP_ANNOUNCEMENT_TEXT_GLOBAL_CHAT");
            titleTextKeyDict.Add(SettingType.KUDO_JACKPOT, "POPUP_ANNOUNCEMENT_TEXT_KUDO_JACKPOT");
            titleTextKeyDict.Add(SettingType.KUDO_TORNAMENT, "POPUP_ANNOUNCEMENT_TEXT_KUDO_TOURNAMENT");
            titleTextKeyDict.Add(SettingType.KUDO_NEW_USER_WELCOME, "POPUP_ANNOUNCEMENT_TEXT_KUDO_NEW_USER_WELCOME");

            isInitDict = true;
        }

        private void SetButton()
        {
            string toggleElementName = CellName + "/Base/On Off/Tab Settings On Off";
            ContextElement toggleElement = ContextUtils.FindElement(agent, toggleElementName, ContextSearchingType.FullNameSearch);

            var toggleProperty = toggleElement as IContextBooleanProperty;
            toggleProperty.SetBooleanProperty(state);

            Animator buttonAnimator = toggleElement.GetComponent<Animator>();
            buttonAnimator.SetBool("Active", state);

            SetButtonListner(ButtonType.ON);
            SetButtonListner(ButtonType.OFF);
            SetButtonListner(ButtonType.BASE);
        }

        private void SetButtonListner(ButtonType type)
        {
            string buttonElementName = string.Empty;
            switch (type)
            {
                case ButtonType.ON:
                    buttonElementName = CellName + "/Base/On Off/Tab Settings On Off/Button On";
                    break;
                case ButtonType.OFF:
                    buttonElementName = CellName + "/Base/On Off/Tab Settings On Off/Button Off";
                    break;
                case ButtonType.BASE:
                    buttonElementName = CellName + "/Base";
                    break;
            }

            ContextElement buttonElement = ContextUtils.FindElement(agent, buttonElementName, ContextSearchingType.FullNameSearch);
            ContextElement onButtonTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);

            switch (type)
            {
                case ButtonType.ON:
                    MetaContextElementUtils.SetTextGlobal(onButtonTextElement, "BUTTON_ON");
                    break;
                case ButtonType.OFF:
                    MetaContextElementUtils.SetTextGlobal(onButtonTextElement, "BUTTON_OFF");
                    break;
            }

            IContextClickable clickableElement = buttonElement as IContextClickable;
            clickableElement.RemoveAllListener();

            GraphOwner owner = null;

            if (ownerSystem != null)
                owner = ownerSystem.agent.GetComponent<GraphOwner>();

            if (owner != null)
                clickableElement.AddListenerOnClick((ContextElement sender) => { owner.SendEvent<ContextElement>(EventName, sender); });
            else
                clickableElement.AddListenerOnClick((ContextElement sender) => { SendEvent<ContextElement>(EventName, sender); });
        }

        private void SetTitle()
        {
            string titleTextElementName = CellName + "/Text";
            ContextElement titleTextElement = ContextUtils.FindElement(agent, titleTextElementName, ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetTextGlobal(titleTextElement, TitleTextKey);
        }
    }
}
