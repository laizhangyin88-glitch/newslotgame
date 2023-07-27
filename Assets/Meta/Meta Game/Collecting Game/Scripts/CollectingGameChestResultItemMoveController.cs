using System.Collections.Generic;
using BagelCode.ClientModels;
using frame8.Logic.Misc.Other.Extensions;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Scratcher
{
    public class CollectingGameChestResultItemMoveController : MonoBehaviour
    {
        private string bundle;
        private string sharedBundle;
        private ContextElement agent;
        private Animator animator;
        private List<Blackboard> resultList;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private int prevMoveItemCount;

        private bool gameWasClosed;

        public void OnInit(bool isShareItem)
        {
            bundle = GetComponent<Blackboard>().GetValue<string>("_bundle");
            sharedBundle = GetComponent<Blackboard>().GetValue<string>("_sharedBundle");
            agent = GetComponent<ContextElement>();
            animator = GetComponent<Animator>();

            resultList = new List<Blackboard>();
            if (isShareItem)
                resultList.AddRange(BlackboardQueryUtils.GetSharePieceList());
            else
                resultList.AddRange(BlackboardQueryUtils.GetResultPieceList());

            ContextElement scratcherItemAreaElement = ContextUtils.FindElement(agent, "Scratcher Item Area", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < resultList.Count; i++)
            {
                MetaObjectUtils.MakePrefab(sharedBundle, "Scratcher Item Position", scratcherItemAreaElement.transform, null, string.Format("Scratcher Item Position {0}", i));
            }
            scratcherItemAreaElement.UpdateContext(true);

            ContextElement buttonScratchEffectElement = ContextUtils.FindElement(agent, "Base/Button Scratch Effect", ContextSearchingType.FullNameSearch);
            buttonScratchEffectElement.gameObject.GetComponentInChildren<OverrideRelativeParticleSortingLayer>().UpdateSortingLayer();

            UpdateScratcher();

            animator.SetBool("Active", false);
        }

        public void MoveItemToScratcher()
        {
            List<Blackboard> itemListToMove = new List<Blackboard>();
            var targetItemList = BlackboardQueryUtils.GetScratcherByItem(resultList[0].GetValue<int>("pieceId")).GetValue<List<Blackboard>>("pieceList");

            for (int i = 0; i < resultList.Count; i++)
            {
                for (int j = 0; j < targetItemList.Count; j++)
                {
                    if (resultList[i].GetValue<int>("pieceId") == targetItemList[j].GetValue<int>("pieceId"))
                    {
                        itemListToMove.Add(resultList[i]);
                        break;
                    }
                }
            }

            prevMoveItemCount = itemListToMove.Count;
            ContextElement parentElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);
            for (int i = 0; i < itemListToMove.Count; i++)
            {
                ContextElement anchorElement = ContextUtils.FindElement(agent, string.Format("Scratcher Item Area/Scratcher Item Position {0}/Anchor", i), ContextSearchingType.FullNameSearch);
                Transform scratcherItem = anchorElement.transform.GetChild(0);

                GameObject itemEffect = MetaObjectUtils.MakePrefab(sharedBundle, "Scratcher Item Effect", scratcherItem);
                itemEffect.SetActive(true);

                Transform[] effects = itemEffect.transform.GetChildren();
                for (int j = 0; j < effects.Length; j++)
                    effects[j].GetComponent<OverrideRelativeParticleSortingLayer>().UpdateSortingLayer();

                int pieceTypeCount = 4;
                int index = BlackboardQueryUtils.GetIndexOfItem(itemListToMove[i].GetValue<int>("pieceId")) % pieceTypeCount;

                ContextElement targetElement = ContextUtils.FindElement(parentElement, string.Format("Scratcher Item 0{0}", index + 1), ContextSearchingType.FullNameSearch);

                int itemId = itemListToMove[i].GetValue<int>("pieceId");

                var from = anchorElement.transform.position;
                var to = targetElement.transform.position;

                scratcherItem.SetParent(parentElement.transform);
                AsyncActionUtils.ApplyMovement(this, scratcherItem.transform, from, to, 0.5f, TweenUtils.VectorTweenCollectMove, i / 10f,
                    () => UpdateItemWhenArrive(itemId, scratcherItem));
            }

            GSManager.Instance.GetHandler("Collecting_Game_Item_Move_With_Effect").Play();
        }

        public void UpdateItemWhenArrive(int itemId, Transform resultItem)
        {
            bool gameClosed = false;

            Blackboard currentScratcher = BlackboardQueryUtils.GetScratcherByItem(resultList[0].GetValue<int>("pieceId"));
            List<Blackboard> itemList = currentScratcher.GetValue<List<Blackboard>>("pieceList");

            for (int i = 0; i < itemList.Count; i++)
            {
                Blackboard itemInfo = itemList[i];

                int currentItemId = itemInfo.GetValue<int>("pieceId");
                int possessions = itemInfo.GetValue<int>("possessions");
                int requirement = itemInfo.GetValue<int>("requirement");

                for (int j = resultList.Count - 1; j >= 0; j--)
                {
                    int openResultItemId = resultList[j].GetValue<int>("pieceId");
                    if (openResultItemId == currentItemId)
                    {
                        if (openResultItemId == itemId)
                            resultList.RemoveAt(j);
                        else
                            possessions -= resultList[j].GetValue<int>("count");
                    }
                }

                string amountText;
                if (possessions < requirement)
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GRAY_TEXT", possessions, requirement);
                    gameClosed = true;
                }
                else
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GREEN_TEXT", possessions, requirement);
                }

                ContextElement scratcherItemElement = ContextUtils.FindElement(agent, string.Format("Base/Scratcher Item 0{0}", i + 1), ContextSearchingType.FullNameSearch);

                ContextElement amountTextElement = ContextUtils.FindElement(scratcherItemElement, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(amountTextElement, amountText);

                ContextElement pieceCoverElement = ContextUtils.FindElement(scratcherItemElement, "Item Area/Cover", ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SetActive(pieceCoverElement, possessions == 0);
            }

            if (!gameClosed && gameWasClosed)
            {
                ContextElement baseElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);

                ContextElement buttonScratchElement = ContextUtils.FindElement(baseElement, "Button Scratch", ContextSearchingType.ChildrenSearch);
                buttonScratchElement.GetComponent<PIDButton>().interactable = true;

//                baseElement.GetComponent<Animator>().SetBool("GameClosed", false);
                baseElement.GetComponent<ContextAnimator>().propertyName = "GameClosed";
                baseElement.GetComponent<ContextAnimator>().isPreserve = true;
                baseElement.GetComponent<ContextAnimator>().SetBooleanProperty(false);

                GSManager.Instance.GetHandler("Collecting_Game_Item_Complete").Play();

                ContextElement buttonScratchEffectElement = ContextUtils.FindElement(baseElement, "Button Scratch Effect", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(buttonScratchEffectElement, true);

                gameWasClosed = false;
            }

            Transform[] children = transform.GetChildren();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name == "Badge Area" || children[i].name == "Effect Result Light")
                {
                    GameObject.Destroy(children[i].gameObject);
                }
            }


        }

        public int GetLeftItemCount()
        {
            return resultList.Count;
        }

        public void ChangeScratcher()
        {
            animator.SetTrigger("Change");

            UpdateScratcher();
        }

        public void MoveBottomItemToLeft()
        {
            for (int i = prevMoveItemCount; i < prevMoveItemCount + resultList.Count; i++)
            {
                ContextElement anchorElement = ContextUtils.FindElement(agent, string.Format("Scratcher Item Area/Scratcher Item Position {0}/Anchor", i), ContextSearchingType.FullNameSearch);
                Transform scratcherItem = anchorElement.transform.GetChild(0);

                ContextElement targetElement = ContextUtils.FindElement(agent, string.Format("Scratcher Item Area/Scratcher Item Position {0}/Anchor", i - prevMoveItemCount), ContextSearchingType.FullNameSearch);

                var from = anchorElement.transform.position;
                var to = targetElement.transform.position;

                scratcherItem.SetParent(targetElement.transform);
                AsyncActionUtils.ApplyMovement(this, scratcherItem, from, to, 0.5f, TweenUtils.VectorTweenCollectMove, (i - prevMoveItemCount) / 10f);
            }

            GSManager.Instance.GetHandler("Collecting_Game_Item_Slide").Play();

            for (int i = prevMoveItemCount + resultList.Count - 1; i >= resultList.Count; i--)
            {
                ContextElement positionElement = ContextUtils.FindElement(agent, string.Format("Scratcher Item Area/Scratcher Item Position {0}", i), ContextSearchingType.FullNameSearch);
                GameObject.Destroy(positionElement.gameObject);
            }
        }

        private void UpdateScratcher()
        {
            if (resultList.Count == 0)
                return;

            Blackboard currentScratcher = BlackboardQueryUtils.GetScratcherByItem(resultList[0].GetValue<int>("pieceId"));
            List<Blackboard> itemList = currentScratcher.GetValue<List<Blackboard>>("pieceList");

            ContextElement baseElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);

            gameWasClosed = false;

            for (int i = 0; i < itemList.Count; i++)
            {
                Blackboard itemInfo = itemList[i];
                ContextElement scratcherItemElement = ContextUtils.FindElement(baseElement, string.Format("Scratcher Item 0{0}", i + 1), ContextSearchingType.ChildrenSearch);

                ContextElement itemAreaElement = ContextUtils.FindElement(scratcherItemElement, "Item Area", ContextSearchingType.ChildrenSearch);

                if (itemAreaElement.ChildCount == 0)
                {
                    MetaObjectUtils.MakePrefab(bundle, "Scratcher Item", itemAreaElement.transform);
                    itemAreaElement.UpdateContext(true);
                }

                ContextElement starElement = ContextUtils.FindElement(itemAreaElement, "Star", ContextSearchingType.ChildrenSearch);

                var rarity = itemInfo.GetValue<ScratcherPieceRarity>("rarity");
                MetaContextElementUtils.SetSprite(starElement, CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);

                ContextElement ItemImageElement = ContextUtils.FindElement(itemAreaElement, "Image", ContextSearchingType.ChildrenSearch);

                int itemId = itemInfo.GetValue<int>("pieceId");
                Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(itemId);
                MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);

                ContextElement amountTextElement = ContextUtils.FindElement(scratcherItemElement, "Text", ContextSearchingType.ChildrenSearch);
                int possessions = itemInfo.GetValue<int>("possessions");

                for (int j = 0; j < resultList.Count; j++)
                {
                    if (resultList[j].GetValue<int>("pieceId") == itemId)
                    {
                        possessions -= resultList[j].GetValue<int>("count");
                        break;
                    }
                }

                if (possessions < 0)
                    possessions = 0;

                int requirement = itemInfo.GetValue<int>("requirement");

                string amountText;
                if (possessions < requirement)
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GRAY_TEXT", possessions, requirement);
                    gameWasClosed = true;
                }
                else
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GREEN_TEXT", possessions, requirement);
                }

                MetaContextElementUtils.SetText(amountTextElement, amountText);

                ContextElement pieceCoverElement = ContextUtils.FindElement(itemAreaElement, "Cover", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(pieceCoverElement, possessions == 0);
            }

            Transform[] children = baseElement.transform.GetChildren();
            for (int j = 0; j < children.Length; j++)
            {
                if (children[j].name == "Scratcher Item")
                {
                    GameObject.Destroy(children[j].gameObject);
                }
            }

            ContextElement buttonScratchElement = ContextUtils.FindElement(baseElement, "Button Scratch", ContextSearchingType.ChildrenSearch);
            buttonScratchElement.GetComponent<PIDButton>().interactable = !gameWasClosed;
            buttonScratchElement.GetComponent<PIDButton>().onClick = new Button.ButtonClickedEvent();
            buttonScratchElement.GetComponent<PIDButton>().scaleFactor = 1.0f;

            ContextElement buttonScratchTextElement = ContextUtils.FindElement(buttonScratchElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonScratchText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_SCRATCH_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonScratchTextElement, buttonScratchText);

//            baseElement.GetComponent<Animator>().SetBool("GameClosed", gameWasClosed);
            baseElement.GetComponent<ContextAnimator>().propertyName = "GameClosed";
            baseElement.GetComponent<ContextAnimator>().isPreserve = true;
            baseElement.GetComponent<ContextAnimator>().SetBooleanProperty(gameWasClosed);

            ContextElement scratcherAreaElement = ContextUtils.FindElement(baseElement, "Scratcher Area", ContextSearchingType.ChildrenSearch);
            scratcherAreaElement.GetComponent<PIDButton>().interactable = false;

            if (scratcherAreaElement.transform.childCount == 0)
            {
                MetaObjectUtils.MakePrefab(sharedBundle, "Scratcher Simple Image", scratcherAreaElement.transform);
                scratcherAreaElement.UpdateContext(true);
            }

            ContextElement simpleImageElement = ContextUtils.FindElement(scratcherAreaElement, "Scratcher Simple Image/Base", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetWebImage(
                simpleImageElement,
                currentScratcher.GetValue<string>("scratcherSimpleImageUrl"),
                CacheType.FileCache,
                false,
                () => {}
            );

            var reward = currentScratcher.GetValue<Blackboard>("reward");
            long maxPrize = reward.GetVariable<long>("maxWinCredit")?.value ?? 0L;

            int version = reward.GetVariable<int>("version")?.value ?? 0;
            var topPrizeArea = ContextUtils.FindElement(scratcherAreaElement, "Scratcher Simple Image/Top Prize Area", ContextSearchingType.FullNameSearch);

            if (version == 0 || maxPrize == 0L)
            {
                topPrizeArea.gameObject.SetActive(false);
            }
            else
            {
                maxPrize = NumberUtils.GetMultiplierNumeratorValue(maxPrize, reward.GetValue<long>("tierMultiplierNumerator"));

                topPrizeArea.gameObject.SetActive(true);

                var simpleImagePrizeTextElement = ContextUtils.FindElement(topPrizeArea, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetTextGlobal(simpleImagePrizeTextElement, "COLLECTING_GAME_SIMPLE_IMAGE_PRIZE_TEXT", maxPrize);
            }

            ContextElement buttonMagnifierElement = ContextUtils.FindElement(baseElement, "Button Magnifier", ContextSearchingType.ChildrenSearch);
            buttonMagnifierElement.GetComponent<PIDButton>().interactable = false;

            ContextElement buttonScratchEffectElement = ContextUtils.FindElement(baseElement, "Button Scratch Effect", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetActive(buttonScratchEffectElement, false);
        }
    }
}
