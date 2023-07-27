using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine.Events;

namespace BagelCode.EpicPass
{
    public class EpicPassV2RewardItemController : MonoBehaviour
    {
        private ContextElement rootElement;

        private ContextElement rewardsElement;

        private ContextElement collectedIconElement;
        private ContextElement collectEffectElement;

        private ContextElement selectOutlineElement;

        private ContextElement coverElement;
        private ContextElement lockElement;

        private bool isInit = false;

        private EpicPassV2RewardCelltemController[] cellItems = null;

        public Blackboard epicPassRewardBB;
        public int rewardLevel;
        public bool isPaidReward;

        private string soundName;

        private UnityAction clickCallBack;

        public void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        public void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rewardsElement = ContextUtils.FindElement(rootElement, "Reward Item List", ContextSearchingType.ChildrenSearch);

            selectOutlineElement = ContextUtils.FindElement(rootElement, "Select Outline", ContextSearchingType.ChildrenSearch);

            collectedIconElement = ContextUtils.FindElement(rootElement, "Collected", ContextSearchingType.ChildrenSearch);
            collectEffectElement = ContextUtils.FindElement(collectedIconElement, "Effect Collect", ContextSearchingType.ChildrenSearch);

            coverElement = ContextUtils.FindElement(rootElement, "Cover", ContextSearchingType.ChildrenSearch);
            lockElement = ContextUtils.FindElement(rootElement, "Locked", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                rootElement,
                () =>
                {
                    OnCollectReward();
                }
            );

            CreateCellItems();

            isInit = true;
        }

        private void CreateCellItems()
        {
            cellItems = new EpicPassV2RewardCelltemController[EpicPassUtilsV2.MAX_REWARD_CELL_ITEM_COUNT];

            for (int i = 0; i < EpicPassUtilsV2.MAX_REWARD_CELL_ITEM_COUNT; ++i)
            {
                GameObject obj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Epic Pass Always Reward Cell Item", rewardsElement.transform);
                if (obj != null)
                {
                    cellItems[i] = obj.GetComponent<EpicPassV2RewardCelltemController>();
                    obj.name = string.Format("Epic Pass Always Reward Cell Item {0}", i);
                    obj.gameObject.SetActive(false);
                }
            }
        }

        public void SetValues(Blackboard epicPassRewardBB, int rewardLevel, bool isPaidReward)
        {
            this.epicPassRewardBB = epicPassRewardBB;
            this.rewardLevel = rewardLevel;
            this.isPaidReward = isPaidReward;
        }

        public void SetCallback(UnityAction callback)
        {
            clickCallBack = callback;
        }

        public void UpdateVariables()
        {
            InitProperty();

            var rewardInfoBB = BlackboardUtils.FindVariable<List<Blackboard>>(epicPassRewardBB, "rewardList");

            if (rewardInfoBB == null || rewardInfoBB.value == null)
            {
                collectedIconElement.gameObject.SetActive(false);
                coverElement.gameObject.SetActive(true);

                if (lockElement != null)
                    lockElement.gameObject.SetActive(false);
            }
            else
            {
                int epicPassLevel = EpicPassUtilsV2.Level;
                bool lockReward = isPaidReward && !EpicPassUtilsV2.Paid;

                bool isClaimed = epicPassRewardBB.GetValue<bool>("isClaimed");

                bool levelLock = epicPassLevel < rewardLevel;
                //bool isCollectable = !levelLock && !isClaimed && !lockReward;
                bool enableCover = isClaimed || levelLock;

                for (int i = 0; i < EpicPassUtilsV2.MAX_REWARD_CELL_ITEM_COUNT; ++i)
                {
                    if (rewardInfoBB.value.Count > i)
                    {
                        if (cellItems[i] != null)
                        {
                            cellItems[i].SetValues(rewardInfoBB.value[i]);
                            cellItems[i].UpdateVariables();
                            cellItems[i].gameObject.SetActive(true);
                        }
                    }
                    else
                        cellItems[i]?.gameObject.SetActive(false);
                }

                collectedIconElement.gameObject.SetActive(isClaimed);
                coverElement.gameObject.SetActive(enableCover);
                selectOutlineElement.gameObject.SetActive(!enableCover);

                RewardType rewardType = (rewardInfoBB == null || rewardInfoBB.value == null || rewardInfoBB.value.Count == 0) ? RewardType.UNKNOWN : rewardInfoBB.value[0].GetValue<RewardType>("type");
                soundName = MetaCommonRewardUtils.GetRewardSoundName(rewardType);

                if (lockElement != null)
                    lockElement.gameObject.SetActive(lockReward);
            }

            collectEffectElement.gameObject.SetActive(false);
        }

        public void OnClaimed()
        {
            Refresh();
        }

        private void Refresh()
        {
            UpdateVariables();
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name == EpicPassUtilsV2.ON_LEVEL_UP_EVENT)
                Refresh();
            else if (eventData.name == EpicPassUtilsV2.ON_REFRESH_EVENT)
                Refresh();
            else if (eventData.name == EpicPassUtilsV2.ON_REWARD_REFRESH)
                Refresh();
        }

        private void OnCollectReward()
        {
            bool isCollectable = !(EpicPassUtilsV2.Level < rewardLevel) && !epicPassRewardBB.GetValue<bool>("isClaimed") && !(isPaidReward && !EpicPassUtilsV2.Paid);
            if (isCollectable == false) return;

            // Play reward sound.
            if (!string.IsNullOrEmpty(soundName))
                GSManager.Instance.GetHandler(soundName).Play();

            // Show Type Effects.
            collectEffectElement.gameObject.SetActive(true);
            selectOutlineElement.gameObject.SetActive(false);
            if (clickCallBack != null)
                clickCallBack.Invoke();

            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<GameObject>("OnCollectReward", gameObject));
        }
    }
}