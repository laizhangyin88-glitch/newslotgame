using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using ParadoxNotion.Design;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class MoveCollectingGameResultItem : ActionTask<ContextElement>
    {
        public BBParameter<GameObject> resultItemMoveScene;
        public BBParameter<ContextElement> scratcherItemAreaElement;
        public BBParameter<bool> isShareItem;

        private int itemToMoveCount;

        protected override string info
        {
            get { return "Start Move Collecting Game Result Item Scene"; }
        }

        protected override void OnExecute()
        {
            List<Blackboard> resultList;
            if (isShareItem.value) 
                resultList = BlackboardQueryUtils.GetSharePieceList();
            else
                resultList = BlackboardQueryUtils.GetResultPieceList();

            for (int i = 0; i < resultList.Count; i++)
            {
                ContextElement positionElement = ContextUtils.FindElement(scratcherItemAreaElement.value, string.Format("Scratcher Item Position {0}", i), ContextSearchingType.ChildrenSearch);
                ContextElement anchorElement = ContextUtils.FindElement(positionElement, "Anchor", ContextSearchingType.ChildrenSearch);
                Transform scratcherItem = anchorElement.transform.GetChild(0);

                ContextElement targetPositionElement = ContextUtils.FindElement(resultItemMoveScene.value.GetComponent<ContextElement>(), 
                    string.Format("Scratcher Item Area/Scratcher Item Position {0}", i), ContextSearchingType.FullNameSearch);
                
                ContextElement targetElement = ContextUtils.FindElement(targetPositionElement, "Anchor", ContextSearchingType.ChildrenSearch);

                var item = scratcherItem.GetComponent<CollectingGameResultItem>();
                var from = anchorElement.transform.position;
                var to = targetElement.transform.position;

                scratcherItem.SetParent(targetElement.transform);
                AsyncActionUtils.ApplyMovement(item, scratcherItem, from, to, 0.5f, TweenUtils.VectorTweenCollectMove, (resultList.Count - i - 1) / 10f, MoveComplete);
            }

            GSManager.Instance.GetHandler("Collecting_Game_Item_Move").Play();

            if (resultList.Count > 0)
                itemToMoveCount = resultList.Count;
            else
                EndAction(true);
        }

        protected override void OnUpdate()
        {
            if (itemToMoveCount == 0)
            {
                resultItemMoveScene.value.GetComponent<Animator>().SetBool("Active", true);
                
                EndAction(true);
            }
        }

        private void MoveComplete()
        {
            itemToMoveCount--;
        }
    }
}