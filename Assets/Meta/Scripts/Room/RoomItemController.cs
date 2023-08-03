using hall;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class RoomItemController : MonoBehaviour
    {
        public int gameId;
        private bool isInit = false;
        public GameObject backgroundObject;
        public RoomStatusInfo roomStatusInfo;
        private ContextElement selfContextElement;
        private ContextElement buttonElemenet;
        private ContextElement enterMinText;

        public void UpdateRoomInfo(RoomStatusInfo roomStatusInfo, bool isUnlock = false)
        {
            InitContext();

            this.roomStatusInfo = roomStatusInfo;

            SetButtonInteractable(false);

            if (roomStatusInfo != null)
            {
                SetCommonValues();
                SetRoomView(isUnlock);
                SetStatus();
            }
        }

        private void SetButtonInteractable(bool isInteractable)
        {
            MetaContextElementUtils.SetBooleanProperty(buttonElemenet, isInteractable);
        }

        private void InitContext()
        {
            backgroundObject.SetActive(false);

            if (isInit) return;

            selfContextElement = gameObject.GetComponent<ContextElement>();
            selfContextElement.UpdateContext(false);

            buttonElemenet = ContextUtils.FindElement(selfContextElement, "Anchor", ContextSearchingType.ChildrenSearch);

            enterMinText = ContextUtils.FindElement(buttonElemenet, "EnterMinText", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        private void SetCommonValues()
        {
            //enterGameInfo.gameId = 0;
            //enterGameInfo.slotStatus = 0;
        }

        private void SetRoomView(bool isRefresh)
        {
            MetaContextElementUtils.SetText(enterMinText, roomStatusInfo.enter_min.ToString());
            backgroundObject.SetActive(true);
        }

        private void SetStatus()
        {
            SetButtonInteractable(true);
        }

        public void OnClick()
        {
            //Test
            //string value = "OnEnterFishGame";
            //gameObject.SendMessage("SendNow", value);

            EnterGameReq msg = new EnterGameReq();
            msg.game_id = roomStatusInfo.game_id;
            msg.room_id = roomStatusInfo.room_id;
            WebSocketManager.Instance.SendHallMessage(HALL_CMD.HALL_CMD_EnterGame_Req, WebSocketTool.Serialize(msg));
        }

    }
}
