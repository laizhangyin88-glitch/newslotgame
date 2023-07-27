using UnityEngine;
using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

using static BagelCode.Tasks.Actions.ClientAPI.InitAnnouncementSetting;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Utils")]
    public class ToggleAnnouncementSetting : ActionTask<ContextElement>
    {
        public BBParameter<SettingType> typeInfo;

        private Animator buttonAnimator;
        private ContextElement toggleElement;

        private string CellName
        {
            get => GetCellName(typeInfo.value);
        }

        private string Key
        {
            get => GetKey(typeInfo.value);
        }

        private bool state;

        protected override string info
        {
            get
            {
                return string.Format("Toggle {0}", typeInfo.value);
            }
        }

        protected override void OnExecute()
        {
            SetToggle();
            RequestSystemOptions();

            EndAction(true);
        }

        private void SetToggle()
        {
            string elementName = CellName + "/Base/On Off/Tab Settings On Off";
            toggleElement = ContextUtils.FindElement(agent, elementName, ContextSearchingType.FullNameSearch);

            var toggleProperty = toggleElement as IContextBooleanProperty;
            state = !toggleProperty.GetBooleanProperty();
            toggleProperty.SetBooleanProperty(state);

            if (buttonAnimator == null)
                buttonAnimator = toggleElement.GetComponent<Animator>();
            buttonAnimator.SetBool("Active", state);

            var settingVar = BlackboardUtils.GetOrCreateVariable<bool>(Key);
            settingVar.value = !settingVar.value;
        }

        private void RequestSystemOptions()
        {
            bool push = BlackboardUtils.GetOrCreateVariable<bool>("/userOptions/pushNotification").value;
            bool globalChat = BlackboardUtils.GetOrCreateVariable<bool>(GetKey(SettingType.GLOBAL_CHAT)).value;
            bool jackpot = BlackboardUtils.GetOrCreateVariable<bool>(GetKey(SettingType.KUDO_JACKPOT)).value;
            bool tournament = BlackboardUtils.GetOrCreateVariable<bool>(GetKey(SettingType.KUDO_TORNAMENT)).value;
            bool newUser = BlackboardUtils.GetOrCreateVariable<bool>(GetKey(SettingType.KUDO_NEW_USER_WELCOME)).value;
            bool pipMode = BlackboardUtils.GetOrCreateVariable<bool>(GetKey(SettingType.PIP_MODE)).value;

            BagelCodeClientAPI.SystemOptionsUpdateRequest(
                push, jackpot, tournament, globalChat, newUser, pipMode,
            (response) =>
            {

            },
            (error) =>
            {

            });
        }
    }
}
