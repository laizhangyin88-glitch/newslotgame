using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class LobbySlotFavoriteController : MonoBehaviour
    {
        public Transform anchor;
        public Transform thumbnailArea;
        public Transform topArea;

        public bool isLong;

        // To do. convert private
        public GameObject thumbObject;
        public GameObject selected;

        private bool isInit = false;
        private bool isSelect = false;

        public int gameID;
        public string gameTitle;

        public Blackboard slotInfoBB;
        public Blackboard gameInfoBB;

        private ContextElement rootElement;
        private EnterGameInfoBehaviour enterGameInfo;

        private Dictionary<string, GameObject> slotThumbDict = new Dictionary<string, GameObject>();

        public void UpdateSlotInfo(Blackboard newSlotInfoBB, Blackboard newGameInfoBB)
        {
            InitContext();
            DestroyPrevObjects();

            slotInfoBB = newSlotInfoBB;
            gameInfoBB = newGameInfoBB;

            if(gameInfoBB != null)
            {
                SetCommonValues(slotInfoBB, gameInfoBB);
                SetSlotImage();
            }
        }

        private void InitContext()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext();

            enterGameInfo = gameObject.GetComponent<EnterGameInfoBehaviour>();
            enterGameInfo.gameId = 0;

            isInit = true;
        }

        private void DestroyPrevObjects()
        {
            if(thumbObject != null) thumbObject.SetActive(false);
        }

        private void SetCommonValues(Blackboard slotInfoBB, Blackboard gameInfoBB)
        {
            gameID = gameInfoBB.GetValue<int>("gameId");
            gameTitle = gameInfoBB.GetValue<string>("gameTitle");

            // 0 Default,1 Comming soon,2 Comming soon,3 Under construction,4 Update to play,5 Early Access
            var statusIndex = BlackboardUtils.FindVariable<int>(slotInfoBB, "flags/status").value;

            enterGameInfo.gameId = gameID;
            enterGameInfo.slotStatus = statusIndex;
        }

        private void SetSlotImage()
        {
            string stringImageKey = isLong ? MetaIconUtils.SLOT_THUMBNAIL_BIG : MetaIconUtils.SLOT_THUMBNAIL_SMALL;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, stringImageKey, gameTitle);

            // Load from pool
            if( LoadSlotImageFromPool(assetName) ) return;

            if( MakeSlotImage(assetName) ) return;
        }

        private bool LoadSlotImageFromPool(string assetName)
        {
            if(slotThumbDict.ContainsKey(assetName))
            {
                thumbObject = slotThumbDict[assetName];
                if(thumbObject != null)
                {
                    thumbObject.SetActive(true);
                    return true;
                }
            }

            return false;
        }

        private bool MakeSlotImage(string assetName)
        {
            thumbObject = MetaIconUtils.MakeSlotImageObjectFromGameTitle(gameTitle, isLong, false, thumbnailArea, "");
            if(thumbObject == null) return false;

            slotThumbDict[assetName] = thumbObject;
            return true;
        }

        public void Select(bool isSelect)
        {
            this.isSelect = isSelect;
            selected.SetActive(isSelect);
        }

        public void EnterGame()
        {
            if (isSelect)
            {
                gameObject.GetComponent<GameSoundPlayer>().PlayGameSound("UI_Button_Normal");
                enterGameInfo.SetEnterGameInfo();
                gameObject.GetComponent<SendEvent>().SendNow("ClickedSlot");
            }
        }
    }
}
