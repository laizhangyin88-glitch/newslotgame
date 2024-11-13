using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
 {
     
     [Category("★ BagelCode/Club")]
     public class UpdateClubShareItem : ActionTask<ContextElement>
     {
        public BBParameter<int> cellIndex;
        public BBParameter<Blackboard> itemInfoBB;
        public BBParameter<EventInfoType> eventType;
        public BBParameter<string> mgBundleName;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<int> saveAsRequestItemID;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private bool isInit = false;

        private ContextElement itemAreaElement;
        private ContextElement gaugeElement;
        private ContextElement gaugeTextElement;
        private ContextElement requestButtonElement;

        private GameObject itemObj = null;

        protected override string info
        {
            get { return "Update Club Rquest Share Item"; }
        }

        protected override void OnExecute()
        {
            if(itemObj != null)
                GameObject.Destroy(itemObj);

            agent.UpdateContext(false);

            itemAreaElement = ContextUtils.FindElement(agent, "Item Area", ContextSearchingType.ChildrenSearch);
            gaugeElement = ContextUtils.FindElement(agent, "Gauge", ContextSearchingType.ChildrenSearch);
            gaugeTextElement = ContextUtils.FindElement(agent, "Gauge/Text", ContextSearchingType.FullNameSearch);
            requestButtonElement = ContextUtils.FindElement(agent, "Button Request", ContextSearchingType.ChildrenSearch);

            saveAsRequestItemID.value = 0;

            MetaContextElementUtils.SetClickable(
                requestButtonElement,
                "OnRequest",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            itemObj = MetaObjectUtils.MakePrefab(mgBundleName.value, "Share Item", itemAreaElement.transform, null, null);
            ContextElement itemElement = itemObj.GetComponent<ContextElement>();
            itemElement.UpdateContext(false);

            int shareCapacity = 0;
            int requirement = 1;
            int progress = 0;

            switch(eventType.value)
            {
                case EventInfoType.COLLECTING_GAME:
                    int itemId = itemInfoBB.value.GetValue<int>("pieceId");
                    shareCapacity = itemInfoBB.value.GetValue<int>("shareCapacity");
                    requirement = itemInfoBB.value.GetValue<int>("requirement");
                    progress = itemInfoBB.value.GetValue<int>("possessions");

                    saveAsRequestItemID.value = itemId;

                    ContextElement starElement = ContextUtils.FindElement(itemElement, "Star", ContextSearchingType.ChildrenSearch);
                
                    var rarity = itemInfoBB.value.GetValue<ScratcherPieceRarity>("rarity");
                    MetaContextElementUtils.SetSprite(starElement, Scratcher.CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);

                    ContextElement ItemImageElement = ContextUtils.FindElement(itemElement, "Image", ContextSearchingType.ChildrenSearch);

                    Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(itemId);
                    MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);
                    break;
            }

            MetaContextElementUtils.SetText(gaugeTextElement, StringTableUtils.GetString(tableType, "POPUP_CLUB_REQUEST_SHARE_CELL_HAVE_COUNT", progress, requirement));
            MetaContextElementUtils.SimpleSetText(requestButtonElement, "Text", StringTableUtils.GetString(tableType, "POPUP_CLUB_REQUEST_SHARE_CELL_BUTTON", shareCapacity));
            MetaContextElementUtils.SetFloatProperty(gaugeElement, (float)progress/(float)requirement);

            UpdateBG();
            
            EndAction();
        }

        private void UpdateBG()
        {
            int cellStyle = cellIndex.value%2;

            for(int i=0; i<bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i==cellStyle);
            }
        }
     }
 }