using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using BSS.Utils;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Chat
{
    [System.Serializable]
    public class ChannelChatGlobal : ChannelChatBase
    {
        public override ChannelType channelType => ChannelType.Global;
        //private TextBalloonParentSystem welcomeSystemParent;

        // Context Elements
        private ContextButton langButton => mContext.FindElement("Bottom Base/Button Language").GetComponent<ContextButton>();
        private IContextText langButtonText => langButton.Find("Text").GetComponent<IContextText>();
        private ContextElement langSelectBase => mContext.FindElement("Bottom Base/Language Select Base");
        private ScrollRect langSelectScrollRect => langSelectBase.transform.GetComponentInChildren<ScrollRect>();
        private ContextButton clubPrButton => ContextUtils.FindElement(mContext, "Buttons/Button Club PR", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();

        protected override void Start()
        {
            base.Start();
            InitLanguageButton();
            InitClubPrButton();
        }

        public override void OnChannelSelected()
        {
            mContext.FindElement("Buttons").gameObject.SetActive(true);
            mContext.FindElement("Bottom Base").gameObject.SetActive(true);
            clubPrButton.transform.parent.gameObject.SetActive(EnableClubPr());
        }

        public override void OnChannelUnselected()
        {
            mContext.FindElement("Buttons").gameObject.SetActive(false);
            mContext.FindElement("Bottom Base").gameObject.SetActive(false);
        }

        private void InitLanguageButton()
        {
            //Language Dropdown List Init
            for (int i = 0; i < ChatMetaManager.Instance.globalChannelInfos.Count; i++)
            {
                var info = ChatMetaManager.Instance.globalChannelInfos[i];
                int index = i;
                var langCell = MetaObjectUtils.MakePrefab<ContextButton>("Language List Cell", langSelectScrollRect.content);
                langCell.Find("Text").GetComponent<IContextText>().SetText(info.channelName);
                langCell.AddListenerOnClick((context) =>
                {
                    if (ChatMetaManager.Instance.globalChannelIndex == index) return;
                    langSelectBase.gameObject.SetActive(false);
                    string preId = ChatMetaManager.Instance.GetChannelID(channelType);
                    ChatMetaManager.Instance.ChangeGlobalChannel(index);
                    owner.SetLoadingSpinner(true);
                    langButtonText.SetText(GetGlobalChannelName(index));
                });
            }


            langButtonText.SetText(GetGlobalChannelName());
            langButton.button.onClick.AddListener(
                () =>
                {
                    if (!langSelectBase.gameObject.activeSelf) {
                        //Dropdown On
                        langSelectBase.gameObject.SetActive(true);
                    } else {
                        //Dropdown Off
                        langSelectBase.gameObject.SetActive(false);
                    }
                }
            );
        }
        
        private void InitClubPrButton()
        {
            var textContext= clubPrButton.Find("Text").GetComponent<IContextText>();
            textContext.SetText("CLUB PR");
            long clubId = BlackboardUtils.FindValue<long>("/me/clubId");
            if (clubId == 0)
            {
                clubPrButton.transform.parent.gameObject.SetActive(false);
            }

            clubPrButton.button.onClick.AddListener(
                () =>
                {
                    if (!EnableClubPr())
                    {
                        clubPrButton.transform.parent.gameObject.SetActive(false);
                        return;
                    }
                    owner.SendChatMessageClubPr();
                }
            );
        }

        private bool EnableClubPr()
        {
            long clubId = BlackboardUtils.FindValue<long>("/me/clubId");
            ClubAuthority authority = BlackboardUtils.FindValue<ClubAuthority>("/clubAuthority");
            if (clubId == 0) return false;
            return authority == ClubAuthority.LEADER || authority == ClubAuthority.COLEADER;
        }

        private string GetGlobalChannelName()
        {
            return GetGlobalChannelName(-1);
        }

        private string GetGlobalChannelName(int channelIndex)
        {
            return ChatMetaManager.Instance.GetGlobalChannelName(channelIndex);
        }
    }
}