using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChattingButtonArea : MonoBehaviour
    {
        public ChattingController owner;
        public ContextElement rootElement;
        private ContextInputField inputField;
        private ContextButton typingButton;
        private ContextButton emoticonButton;
        private ContextButton hiButton;
        private ContextButton congrateButton;
        private ContextButton thankYouButton;
        private ContextElement emoticonListBase;
        private ContextButton emoticonBackgroundButton;

        private ContextElement clubPrButton;

        private void Awake()
        {
            var coolTimer = gameObject.AddComponent<ChattingButtonCoolTimer>();
            coolTimer.owner = this;
        }

        private void Start()
        {
            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext();

            inputField = rootElement.FindElement<ContextInputField>("InputField");
            typingButton = rootElement.FindElement<ContextButton>("Button Typing");
            emoticonButton = ContextUtils.FindElement(rootElement, "Button Emoticon", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            hiButton = ContextUtils.FindElement(rootElement, "Button Hi", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            congrateButton = ContextUtils.FindElement(rootElement, "Button Congrate", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            thankYouButton = ContextUtils.FindElement(rootElement, "Button Thank You", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();
            emoticonListBase = owner.rootElement.Find("Emoticon List Base");
            emoticonBackgroundButton = emoticonListBase.FindElement<ContextButton>("Background Button");
            clubPrButton = ContextUtils.FindElement(rootElement, "Button Club PR", ContextSearchingType.FullNameSearch);

            owner.OnChannelChanged += (channelType) =>
            {
                emoticonListBase.gameObject.SetActive(false);
            };

            //Buttons Init
            emoticonButton.RemoveAllListener();
            emoticonButton.AddListenerOnClick(
                (context) =>
                {
                    emoticonListBase.gameObject.SetActive(!emoticonListBase.gameObject.activeSelf);
                }
            );

            emoticonBackgroundButton.RemoveAllListener();
            emoticonBackgroundButton.AddListenerOnClick(
                (context) =>
                {
                    emoticonListBase.gameObject.SetActive(false);
                }
            );

            // if (TouchScreenKeyboard.isSupported)
            // {
                hiButton.Find("Text").GetComponent<ContextTextMeshProUGUI>().SetText("<size=24>Hi");
                hiButton.AddListenerOnClick((context) =>
                {
                    int r = UnityEngine.Random.Range(1, 6);
                    owner.SendChatMessageInstant((InstantChat)r);
                });

                congrateButton.Find("Text").GetComponent<ContextTextMeshProUGUI>().SetText("<size=24>Congrats");
                congrateButton.AddListenerOnClick((context) =>
                {
                    int r = UnityEngine.Random.Range(6, 11);
                    owner.SendChatMessageInstant((InstantChat)r);
                }
                );

                thankYouButton.Find("Text").GetComponent<ContextTextMeshProUGUI>().SetText("<size=24>Thanks");
                thankYouButton.AddListenerOnClick((context) =>
                {
                    int r = UnityEngine.Random.Range(11, 14);
                    owner.SendChatMessageInstant((InstantChat)r);
                });
            // }

            //Input Field Init
            typingButton.button.onClick.AddListener(() =>
            {
                inputField.inputField.ActivateInputField();
                inputField.inputField.Select();
                owner.SetTouchKeyboarding();
            });
        }

        // Unity Interactable Bug. 
        public void UnityBugFunc()
        {
            MetaContextElementUtils.SetBooleanProperty(emoticonButton, false);
            MetaContextElementUtils.SetBooleanProperty(hiButton, false);
            MetaContextElementUtils.SetBooleanProperty(congrateButton, false);
            MetaContextElementUtils.SetBooleanProperty(thankYouButton, false);
            MetaContextElementUtils.SetBooleanProperty(emoticonListBase, false);
            MetaContextElementUtils.SetBooleanProperty(emoticonBackgroundButton, false);
            if(clubPrButton != null)
                MetaContextElementUtils.SetBooleanProperty(clubPrButton, false);

            MetaContextElementUtils.SetBooleanProperty(emoticonButton, true);
            MetaContextElementUtils.SetBooleanProperty(hiButton, true);
            MetaContextElementUtils.SetBooleanProperty(congrateButton, true);
            MetaContextElementUtils.SetBooleanProperty(thankYouButton, true);
            MetaContextElementUtils.SetBooleanProperty(emoticonListBase, true);
            MetaContextElementUtils.SetBooleanProperty(emoticonBackgroundButton, true);
            if(clubPrButton != null)
                MetaContextElementUtils.SetBooleanProperty(clubPrButton, true);
        }
    }
}