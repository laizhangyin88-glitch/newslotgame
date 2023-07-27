using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace BagelCode.Scratcher
{
    public class ScratcherPlayController : MonoBehaviour
    {
        // Settings
        public static float autoDelay = 0.5f;
        public static float waitDelay = 2.5f;
        public static float firstWaitDelay = 5f;
        //

        public Blackboard scratcherInfo;

        private ContextElement root;
        private Blackboard bb;

        public List<ScratcherCellController> cellInstanceList;

        private List<ScratcherPlayGroup> scratcherPlayGroups;
        private RewardScratcherRule scratcherRule;

        private int currentScratcherGroupIndex;

        private bool isInit = false;

        public void InitializeScratcherPlayGroups()
        {
            if (isInit) return;

            root = GetComponent<ContextCompositor>();
            bb = GetComponent<Blackboard>();
            scratcherPlayGroups = new List<ScratcherPlayGroup>();
            scratcherRule = scratcherInfo.GetValue<RewardScratcherRule>("scratcherRule");

            var scratcherObj = bb.GetValue<ContextElement>("_popupScratcher");
            var scratcherBB = scratcherObj.GetComponent<Blackboard>();
            bool isReward = scratcherBB.GetVariable<bool>("_isReward")?.value ?? false;
            BlackboardUtils.SetOrCreateValue(scratcherInfo, "_isReward", isReward);

            PopupScratcherUtils.InitScratcherPlayGroups(scratcherRule, scratcherPlayGroups, scratcherInfo, cellInstanceList, root);

            switch (scratcherRule)
            {
                case RewardScratcherRule.BINGO_DEFAULT:
                case RewardScratcherRule.BINGO_MULTIPLIER:
                    autoDelay = 0.35f;
                    break;
            }

            bb.AddVariable("autoDelay", autoDelay);
            bb.AddVariable("waitDelay", waitDelay);
            bb.AddVariable("firstWaitDelay", firstWaitDelay);

            isInit = true;
        }

        public void PlayGroup()
        {
            StartCoroutine(PlayGroupCoroutine());
        }

        private IEnumerator PlayGroupCoroutine()
        {
            if (currentScratcherGroupIndex < scratcherPlayGroups.Count)
            {
                ScratcherPlayGroup group = scratcherPlayGroups[currentScratcherGroupIndex++];

                if (group.IsEventNameNullorEmpty())
                {
                    bool isLast = group == scratcherPlayGroups.Last();
                    if (isLast) group.SetEventName(ScratcherPlayGroup.EventType.FINISH_SCRATCHER);
                    else
                    {
                        if (group.isPlayNextInstantly)
                            group.SetEventName(ScratcherPlayGroup.EventType.PLAY_NEXT_INSTANT);
                        else
                            group.SetEventName(ScratcherPlayGroup.EventType.PLAY_NEXT);
                    }
                }

                yield return StartCoroutine(group.Play(this));
            }
        }
    }
}
