using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishPlayerInfoPanel
    {
        private FishGameUIManager gameUIManager;
        private FishGameData gameData;
        public List<GameObject> PlayerSeatList;
        public List<GameObject> WaitPlayerBGList;
        private int playerCount;
        public FishPlayerInfoPanel(GameObject gameObj)
        {
            InitData();
            InitView(gameObj);
        }

        private void InitData()
        {
            gameUIManager = FishGameUIManager.Instance;
            gameData = gameUIManager.gameData;
            PlayerSeatList = new List<GameObject>();
            WaitPlayerBGList = new List<GameObject>();
            playerCount = gameData.playerTotalCount;
        }

        private void InitView(GameObject gameObj)
        {
            FindView(gameObj);
        }

        private void FindView(GameObject gameObj)
        {
            FindPlayerSeatView(gameObj.transform);
        }

        private void FindPlayerSeatView(Transform trans)
        {
            for (int i = 1; i < playerCount + 1; i++)
            {
                GameObject gameObj = trans.Find("P" + i).gameObject;
                PlayerSeatList.Add(gameObj);
            }
            for (int i = 1; i < playerCount + 1; i++)
            {
                GameObject gameObj = trans.Find("P" + i + "/WaitPlayerBG").gameObject;
                gameObj.SetActive(true);
                WaitPlayerBGList.Add(gameObj);
            }
        }

        public void IsShowWaitPlayerBG(int index, bool isDisplay)
        {
            WaitPlayerBGList[index].SetActive(isDisplay);
        }

        public void ResetAllSeatWaitPlayerBG()
        {
            for(int i = 0;i < WaitPlayerBGList.Count; i++)
            {
                WaitPlayerBGList[i].SetActive(true);
            }
        }
    }
}
