using BagelCode.ClientModels;
using hall;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class InRoomController : EventMonoBehaviour
    {
        private ContextElement root;
        private GraphOwner owner;

        private ContextElement returnButtonElement;
        private ContextElement tipsButtonElement;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        public void InitProperty()
        {
            owner = GetComponent<GraphOwner>();
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            returnButtonElement = ContextUtils.FindElement(root, "Button Return", FULL);
            tipsButtonElement = ContextUtils.FindElement(root, "Button Tips", FULL);

            MetaContextElementUtils.SetClickable(returnButtonElement, () => owner.SendEvent("OnClose"));
            MetaContextElementUtils.SetClickable(tipsButtonElement, () => Debug.Log("Click my tipsBtn"));
            AddEventListenner();

        }

        private void AddEventListenner()
        {
            WebSocketTool.RegisterReceiveHandler(HALL_CMD.HALL_CMD_EnterGame_Rsp.ToString(), OnEnterGameRsp);
        }


        private void RemoveEvenetListenner()
        {
            WebSocketTool.UnRegisterHandler(HALL_CMD.HALL_CMD_EnterGame_Rsp.ToString(), OnEnterGameRsp);
        }

        private void OnEnterGameRsp(byte[] bytes)
        {
            EnterGameRsp enterGameRsp = WebSocketTool.Deserialize<EnterGameRsp>(bytes);
            if (enterGameRsp.result == 0)
            {
                string value = "OnEnterFishGame";
                gameObject.SendMessage("SendNow", value);
            }
            else
            {
                Debug.LogError("进入房间失败!");
            }
        }

        public void ShowFishGameBubble()
        {
            FishGameManager.Instance.ShowBubble();
        }

        public void LeaveRoom()
        {
            FishGameManager.Instance.LeaveGame();
        }

        private void OnDestroy()
        {
            RemoveEvenetListenner();
        }
    }
}

