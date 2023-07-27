using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class UpdateClubAllTierInfo : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> clubLeagueTierResponseBB;
        public BBParameter<Blackboard> clubTierInfoBB;

        public BBParameter<List<GameObject>> tierGroupList;
        public BBParameter<List<GameObject>> tierList;

        private bool[] lockGroups;
        private int openedTierGroup;
        private int myClubTierGroup;
        private int myClubTier;
        private int maxOpenedLeagueTier;

        private const string ANIMATOR_LOCK = "IsLock";
        private const string ANIMATOR_HIGHLIGHT = "IsHighlight";
        private const string PRIZE_TEXT_KEY = "POPUP_ALL_TIERS_PRIZE_TEXT";

        protected override string info
        {
            get { return "Update Club All Tier Info"; }
        }

        protected override void OnExecute()
        {
            maxOpenedLeagueTier = clubLeagueTierResponseBB.value.GetValue<int>("maxOpenedLeagueTier");
            openedTierGroup = ClubUtils.GetClubTierGroup(maxOpenedLeagueTier);

            myClubTier = clubTierInfoBB.value.GetValue<int>("leagueTier");
            myClubTierGroup = ClubUtils.GetClubTierGroup(myClubTier);

            UpdateGroups();
            UpdatePrizeCells();

            EndAction();
        }

        private void UpdateGroups()
        {
            lockGroups = new bool[tierGroupList.value.Count];

            for(int i=0; i<tierGroupList.value.Count; ++i)
            {
                bool isOpened = i <= openedTierGroup;
                lockGroups[i] = isOpened;

                Animator ani = tierGroupList.value[i].GetComponent<Animator>();
                if (ani == null)
                    continue;

                ani.SetBool(ANIMATOR_LOCK, !isOpened);
                ani.SetBool(ANIMATOR_HIGHLIGHT, myClubTierGroup == i);
            }
        }

        private void UpdatePrizeCells()
        {
            var tierStatusList = clubLeagueTierResponseBB.value.GetValue<List<Blackboard>>("tierList");
            var clubMultiplier = ClubUtils.GetClubRewardMultiplierNumerator();

            for (int i=0; i<tierList.value.Count; ++i)
            {
                if(tierStatusList.Count <= i) break;

                int currentTierGroup = ClubUtils.GetClubTierGroup(i);
                bool isOpened = lockGroups[currentTierGroup];

                Animator ani = tierList.value[i].GetComponent<Animator>();
                if (ani == null)
                    continue;

                ani.SetBool(ANIMATOR_LOCK, isOpened && maxOpenedLeagueTier < i);
                ani.SetBool(ANIMATOR_HIGHLIGHT, myClubTier == i);

                ContextElement element = ContextUtils.FindElement(tierList.value[i].GetComponent<ContextElement>(), "Text Prize", ContextSearchingType.ChildrenSearch);
                IContextText textElement = element as IContextText;
                if(textElement != null)
                {
                    textElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, PRIZE_TEXT_KEY, NumberUtils.GetMultiplierNumeratorValue(tierStatusList[i].GetValue<long>("firstPlaceReward"), clubMultiplier)));
                }
            }
        }
    }
}
