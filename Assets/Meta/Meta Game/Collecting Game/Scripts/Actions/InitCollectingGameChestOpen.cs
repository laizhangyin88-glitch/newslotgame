using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameChestOpen : ActionTask<ContextElement>
    {
        public BBParameter<int> chestCount;
        public BBParameter<int> preservedCount;
        public BBParameter<int> chestId;
        public BBParameter<string> sharedBundle;
        public BBParameter<ContextElement> badgeElement;
        
        private string ON_OPEN_CHEST_EVENT = "OnOpenChest";

        protected override string info
        {
            get { return "Init Collecting Game Chest Open Scene"; }
        }

        protected override void OnExecute()
        {
            preservedCount.value = chestCount.value;
            
            ContextElement chestBaseElement = ContextUtils.FindElement(agent, "Anchor/Base", ContextSearchingType.FullNameSearch);
            Sprite chestSprite = CollectingGameChestData.Instance.chestAssets.assets[BlackboardQueryUtils.GetChestIndex(chestId.value)];
            MetaContextElementUtils.SetSprite(chestBaseElement, chestSprite);

            ContextElement fullCoverButtonElement = ContextUtils.FindElement(agent, "Full Cover Button", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                fullCoverButtonElement,
                ON_OPEN_CHEST_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            agent.GetComponent<Animator>().SetBool("Active", true);

            ContextElement effectOpenElement = ContextUtils.FindElement(agent, "Anchor/Effect Open", ContextSearchingType.FullNameSearch);
            int chestIndex = BlackboardQueryUtils.GetChestIndex(chestId.value);
            GameObject effectOpen = null;
            switch (chestIndex)
            {
                case 0:
                    effectOpen = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Open Chest Bronze", effectOpenElement.transform);
                    break;
                case 1:
                    effectOpen = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Open Chest Silver", effectOpenElement.transform);
                    break;
                case 2:
                    effectOpen = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Open Chest Gold", effectOpenElement.transform);
                    break;
                case 3:
                    effectOpen = MetaObjectUtils.MakePrefab(sharedBundle.value, "Effect Open Chest Diamond", effectOpenElement.transform);
                    break;
            }

            if (effectOpen != null)
            {
                OverrideRelativeParticleSortingLayer[] overriders = effectOpen.GetComponentsInChildren<OverrideRelativeParticleSortingLayer>();
                for (int i = 0; i < overriders.Length; i++)
                    overriders[i].UpdateSortingLayer();
            }

            if (chestCount.value > 1)
            {
                ContextElement badgeAreaElement = ContextUtils.FindElement(agent, "Anchor/Badge Area", ContextSearchingType.FullNameSearch);
                MetaObjectUtils.UpdateBadge(badgeAreaElement, false, chestCount.value);
                
                badgeElement.value = ContextUtils.FindElement(badgeAreaElement, "Badge", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(badgeElement.value, false);
            }
            
            if (chestCount.value > 6)
                chestCount.value = 6;
            
            EndAction(true);
        }
    }
}
