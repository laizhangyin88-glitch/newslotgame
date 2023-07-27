using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ButtonChatController : MonoBehaviour
    {
        public enum PlaceType
        {
            Lobby,
            Club,
            InGame
        }

        public ChannelType channelType;
        public PlaceType placeType;
        private ContextButton contextButton => GetComponent<ContextButton>();
        private Animator animator => GetComponent<Animator>();

        private Coroutine waitCoroutine;

        private bool isInitLastChannelType = false;

        private void OnEnable()
        {
            if(!isInitLastChannelType)
            {
                switch(placeType)
                {
                    case PlaceType.Lobby:
                        ChangeLastChannelType(ChannelType.Global);
                        break;
                    case PlaceType.Club:
                        ChangeLastChannelType(ChannelType.Club);
                        break;
                    case PlaceType.InGame:
                        ChangeLastChannelType(ChannelType.Game);
                        break;
                }

                isInitLastChannelType = true;
            }
        }

        private void Start()
        {
            contextButton.UpdateContext();

            contextButton.AddListenerOnClick(
                (context) =>
                {
                    BI_client_click_chat();
                    var chattingController = FindObjectOfType<ChattingController>();

                    if (chattingController == null)
                    {
                        MetaObjectUtils.MakeScene("Chatting Scene", PopupManager.Instance.transform.Find("Interaction"));
                        chattingController = FindObjectOfType<ChattingController>();
                    }

                    if (placeType == PlaceType.Lobby)
                    {
                        if (!chattingController.IsOpen)
                        {
                            OpenController(chattingController);
                        }
                        else
                        {
                            chattingController.Close();
                        }
                    }
                    else
                    {
                        if(waitCoroutine != null)
                        {
                            StopCoroutine(waitCoroutine);
                            waitCoroutine = null;
                        }

                        if (chattingController.IsOpen)
                        {
                            animator.SetBool("Active", false);
                            chattingController.Close();
                        }
                        else
                        {
                            animator.SetBool("Active", true);
                            OpenController(chattingController);
                            waitCoroutine = StartCoroutine(WaitCloseAsync(chattingController));
                        }
                    }
                }
            );
        }

        private void ChangeLastChannelType(ChannelType channelType)
        {
            ChattingController.LastChannelType = channelType;
        }

        private void OpenController(ChattingController controller)
        {
            StartCoroutine(controller.OpenAsync(ChattingController.LastChannelType));
        }

        private IEnumerator WaitCloseAsync(ChattingController chattingController)
        {
            yield return new WaitUntil(
                ()=> chattingController != null && !chattingController.IsOpen
            );

            animator.SetBool("Active", false);
        }

        private void BI_client_click_chat()
        {
            string place = placeType.ToString().ToLower();
            if (place == "ingame")
                place = "in-game";

            BiEventUtils.ClickChatButton(place);
        }
    }
}
