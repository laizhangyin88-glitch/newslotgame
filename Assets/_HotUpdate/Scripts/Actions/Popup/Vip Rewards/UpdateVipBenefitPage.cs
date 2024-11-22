using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Popup/Vip Rewards")]
    public class UpdateVipBenefitPage : ActionTask<Blackboard>
    {
        public BBParameter<int> meTier;
        public BBParameter<int> targetPageIndex;
        public BBParameter<List<int>> pageTierList;

        public BBParameter<List<GameObject>> pageBenefitCellList;

        private int currentTier = -1;
        private int nextTier = -1;
        private int maxTier = -1;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private bool isInit = false;

        private ContextElement leftTierNameElement;
        private ContextElement leftTierNameTextElement;
        private ContextElement leftTierNameCoverElement;

        private ContextElement rightTierNameAreaElement;
        private ContextElement rightTierNameElement;
        private ContextElement rightTierNameTextElement;
        private ContextElement rightTierNameCoverElement;

        private ContextElement arrowRight;
        private ContextElement arrowLeft;

        private ContextElement dotAreaElement;

        private ContextElement scrollContentsAreaElement;

        private List<string> currentBenefitText = new List<string>();
        private List<string> nextBenefitText = new List<string>();

        private const string benefitCellPrefabName = "VIP Rewards Tier Chart Cell";
        private const int MAX_BENEFIT_COUNT = 9;
        private const string ARROW_DISABLE_STATE_NAME = "IsDisabled";

        protected override void OnExecute()
        {
            if (!isInit)
                InitProperty();

            currentTier = -1;
            nextTier = -1;

            if (targetPageIndex.value < 0)
                targetPageIndex.value = 0;
            else if (targetPageIndex.value >= pageTierList.value.Count)
                targetPageIndex.value = pageTierList.value.Count - 1;

            UpdateTier();
            MetaContextElementUtils.SetIntProperty(dotAreaElement, targetPageIndex.value);

            EndAction();
        }

        private void InitProperty()
        {
            ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

            leftTierNameElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Current Tier Text Area/Current Tier Text", ContextSearchingType.FullNameSearch);
            leftTierNameTextElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Current Tier Text Area/Current Tier Text/Current Tier Text", ContextSearchingType.FullNameSearch);
            leftTierNameCoverElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Current Tier Text Area/Current Tier Text/Cover", ContextSearchingType.FullNameSearch);

            rightTierNameAreaElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Text Area", ContextSearchingType.FullNameSearch);
            rightTierNameElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Text Area/Next Tier Text", ContextSearchingType.FullNameSearch);
            rightTierNameTextElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Text Area/Next Tier Text/Current Tier Text", ContextSearchingType.FullNameSearch);
            rightTierNameCoverElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Next Tier Text Area/Next Tier Text/Cover", ContextSearchingType.FullNameSearch);

            arrowRight = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Arrow Right", ContextSearchingType.FullNameSearch);
            arrowLeft = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Arrow Left", ContextSearchingType.FullNameSearch);

            dotAreaElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Dots Area", ContextSearchingType.FullNameSearch);

            scrollContentsAreaElement = ContextUtils.FindElement(agentElement, "VIP Rewards Pages/Anchor/Scroll/Contents", ContextSearchingType.FullNameSearch);

            // Make Benefit Cells. Reuse Objects. 
            if (pageBenefitCellList.value == null)
                pageBenefitCellList.value = new List<GameObject>();

            if (pageBenefitCellList.value.Count == 0)
            {
                for (int i = 0; i < MAX_BENEFIT_COUNT; ++i)
                {
                    var go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, benefitCellPrefabName, scrollContentsAreaElement.transform, "");
                    go.name = string.Format("Benefit {0}", i);

                    ContextElement cellElement = go.GetComponent<ContextElement>();
                    cellElement.UpdateContext(false);

                    int cellStyle = i % 2;

                    var currentBase01 = ContextUtils.FindElement(cellElement, "Current Tier/Base Color 01", ContextSearchingType.FullNameSearch);
                    var currentBase02 = ContextUtils.FindElement(cellElement, "Current Tier/Base Color 02", ContextSearchingType.FullNameSearch);

                    var nextBase01 = ContextUtils.FindElement(cellElement, "Next Tier/Base Color 01", ContextSearchingType.FullNameSearch);
                    var nextBase02 = ContextUtils.FindElement(cellElement, "Next Tier/Base Color 02", ContextSearchingType.FullNameSearch);

                    currentBase01.gameObject.SetActive(cellStyle == 0);
                    currentBase02.gameObject.SetActive(cellStyle == 1);

                    nextBase01.gameObject.SetActive(cellStyle == 0);
                    nextBase02.gameObject.SetActive(cellStyle == 1);

                    pageBenefitCellList.value.Add(go);
                }
            }

            maxTier = TierUtils.GetMaxTier();

            isInit = true;
        }

        private void UpdateTier()
        {
            currentTier = pageTierList.value[targetPageIndex.value];

            if (targetPageIndex.value == 0 && currentTier + 1 == pageTierList.value[targetPageIndex.value + 1])
            {
                nextTier = -1;
            }
            else
            {
                nextTier = maxTier > currentTier ? currentTier + 1 : -1;
            }

            int currentTierGroup = TierUtils.GetTierGroup(currentTier);

            MetaContextElementUtils.SetBlackboardValue<int>(leftTierNameElement, "tierGroup", currentTierGroup);
            MetaContextElementUtils.SetIntProperty(leftTierNameElement, currentTierGroup);

            // if(currentTier == meTier.value)
            // {
            MetaContextElementUtils.SetText(leftTierNameTextElement, StringTableUtils.GetString(tableType, "VIP_BENEFITS_MY_TIER_TEXT", currentTier));
            // }
            // else
            // {
            //     MetaContextElementUtils.SetText(leftTierNameTextElement, StringTableUtils.GetString(tableType, "VIP_BENEFITS_OTHER_TIER_TEXT", currentTier));
            // }

            rightTierNameAreaElement.gameObject.SetActive(nextTier != -1);

            leftTierNameCoverElement.gameObject.SetActive(currentTier > meTier.value);
            rightTierNameCoverElement.gameObject.SetActive(nextTier > meTier.value);

            if (nextTier != -1)
            {
                int nextTierGroup = TierUtils.GetTierGroup(nextTier);
                MetaContextElementUtils.SetBlackboardValue<int>(rightTierNameElement, "tierGroup", nextTierGroup);
                MetaContextElementUtils.SetIntProperty(rightTierNameElement, nextTierGroup);
                // MetaContextElementUtils.SetText(rightTierNameTextElement, StringTableUtils.GetString(tableType, "VIP_BENEFITS_OTHER_TIER_TEXT", nextTier));
                MetaContextElementUtils.SetText(rightTierNameTextElement, StringTableUtils.GetString(tableType, "VIP_BENEFITS_MY_TIER_TEXT", nextTier));
            }

            MetaContextElementUtils.SetBooleanProperty(arrowLeft, currentTier > 0);
            MetaContextElementUtils.SetBooleanProperty(arrowRight, currentTier + 1 < maxTier);

            UpdateBenefits(currentTier, nextTier);
        }

        private void UpdateBenefits(int currentTier, int nextTier)
        {
            double currentTierMultiplier = TierUtils.GetTierMultiplier(currentTier);
            long currentTimeBonus = TierUtils.GetTimeBonusCoins(currentTier);
            long currentVipCoins = TierUtils.GetVipCoins(currentTier);

            string currentValueColorText = GetBenefitValueColorText(currentTier);

            currentBenefitText.Clear();
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", currentValueColorText, currentTierMultiplier));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", currentValueColorText, currentTierMultiplier));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_ADD_COIN", currentValueColorText, currentTimeBonus));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_FIXED_COIN", currentValueColorText, currentVipCoins));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", currentValueColorText, currentTierMultiplier));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", currentValueColorText, currentTierMultiplier));
            currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", currentValueColorText, currentTierMultiplier));

            // Additional Benefits
            int vipDealV2MinTier = VipDealV2.Utils.GetMinTier();
            int vipDealMinTier = BlackboardUtils.FindValue<int>("/values/tier/VIP_DEAL_MIN_TIER");
            bool isVipDealV2Enabled = VipDealV2.Utils.IsEnabled();
            if (isVipDealV2Enabled) // v2
            {
                if (currentTier >= vipDealV2MinTier)
                {
                    currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", currentValueColorText));
                    currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", currentValueColorText));
                }
            }
            else
            {
                if (currentTier >= vipDealMinTier)
                    currentBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", currentValueColorText));
            }

            nextBenefitText.Clear();
            if (nextTier > 0)
            {
                double nextTierMultiplier = TierUtils.GetTierMultiplier(nextTier);
                long nextTimeBonus = TierUtils.GetTimeBonusCoins(nextTier);
                long nextVipCoins = TierUtils.GetVipCoins(nextTier);
                string nextValueColorText = GetBenefitValueColorText(nextTier);

                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", nextValueColorText, nextTierMultiplier));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", nextValueColorText, nextTierMultiplier));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_ADD_COIN", nextValueColorText, nextTimeBonus));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_FIXED_COIN", nextValueColorText, nextVipCoins));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", nextValueColorText, nextTierMultiplier));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", nextValueColorText, nextTierMultiplier));
                nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_MULTIPLIER", nextValueColorText, nextTierMultiplier));

                // Additional Benefits
                if (isVipDealV2Enabled)
                {
                    if (nextTier >= vipDealV2MinTier)
                    {
                        nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", nextValueColorText));
                        nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", nextValueColorText));
                    }
                }
                else
                {
                    if (nextTier >= vipDealMinTier)
                        nextBenefitText.Add(StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_OPEN", nextValueColorText));
                }
            }

            for (int i = 0; i < MAX_BENEFIT_COUNT; ++i)
            {
                ContextElement cellElement = pageBenefitCellList.value[i].GetComponent<ContextElement>();

                var currentBenefitName = ContextUtils.FindElement(cellElement, "Current Tier/Text Status", ContextSearchingType.FullNameSearch);
                var currentBenefitCover = ContextUtils.FindElement(cellElement, "Current Tier/Cover", ContextSearchingType.FullNameSearch);
                var nextBenefitName = ContextUtils.FindElement(cellElement, "Next Tier/Text Status", ContextSearchingType.FullNameSearch);
                var nextBenefitCover = ContextUtils.FindElement(cellElement, "Next Tier/Cover", ContextSearchingType.FullNameSearch);

                string benefitNameKey = string.Format("VIP_BENEFITS_CELL_{0}", i);
                string currentTitleColorText = GetBenefitTitleColorText(currentTier);
                string nextTitleColorText = GetBenefitTitleColorText(nextTier);

                // currentBenefitCover.gameObject.SetActive(currentTier != meTier.value);
                // nextBenefitCover.gameObject.SetActive(true);
                currentBenefitCover.gameObject.SetActive(currentTier > meTier.value);
                nextBenefitCover.gameObject.SetActive(nextTier > meTier.value);

                string currentBenefixText = "";
                string nextBenefixText = "";

                if (isVipDealV2Enabled && i == 8) // pay deal
                {
                    long maxMultiplierNumerator = VipDealV2.Defines.DEFAULT_PAY_DEAL_MAX_MULTIPLIER_NUMERATOR;
                    double multiplier = NumberUtils.GetMultiplierFromNumerator(maxMultiplierNumerator);

                    currentBenefixText = StringTableUtils.GetString(tableType, benefitNameKey, currentTitleColorText, multiplier);
                    nextBenefixText = StringTableUtils.GetString(tableType, benefitNameKey, nextTitleColorText, multiplier);
                }
                else
                {
                    currentBenefixText = StringTableUtils.GetString(tableType, benefitNameKey, currentTitleColorText);
                    nextBenefixText = StringTableUtils.GetString(tableType, benefitNameKey, nextTitleColorText);
                }

                MetaContextElementUtils.SetText(currentBenefitName, currentBenefixText);
                MetaContextElementUtils.SetText(nextBenefitName, nextBenefixText);

                SetBenefitText(pageBenefitCellList.value[i],
                                currentBenefitText.Count > i ? currentBenefitText[i] : null,
                                nextBenefitText.Count > i ? nextBenefitText[i] : null
                              );
            }
        }

        private void SetBenefitText(GameObject benefitCell, string currentText, string nextText)
        {
            bool hideCurrent = string.IsNullOrEmpty(currentText);
            bool hideNext = string.IsNullOrEmpty(nextText);

            if (hideCurrent && hideNext)
            {
                benefitCell.SetActive(false);
                return;
            }
            else
            {
                benefitCell.SetActive(true);
            }

            ContextElement cellElement = benefitCell.GetComponent<ContextElement>();
            var currentTierElement = ContextUtils.FindElement(cellElement, "Current Tier", ContextSearchingType.ChildrenSearch);
            var emptyElement = ContextUtils.FindElement(cellElement, "Empty", ContextSearchingType.ChildrenSearch);

            emptyElement.gameObject.SetActive(hideCurrent);
            currentTierElement.gameObject.SetActive(!hideCurrent);
            if (!hideCurrent)
            {
                var currentBenefitTextElement = ContextUtils.FindElement(currentTierElement, "Text Benefits", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(currentBenefitTextElement, currentText);
            }

            var nextTierElement = ContextUtils.FindElement(cellElement, "Next Tier", ContextSearchingType.ChildrenSearch);
            nextTierElement.gameObject.SetActive(!hideNext);
            if (!hideNext)
            {
                var nextBenefitTextElement = ContextUtils.FindElement(nextTierElement, "Text Benefits", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(nextBenefitTextElement, nextText);
            }
        }

        private string GetBenefitTitleColorText(int targetTier)
        {
            if (targetTier > meTier.value)
                return StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_COLOR_PREV_TIER");
            // if(targetTier != meTier.value)
            //     return StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_COLOR_PREV_TIER");
            return "";
        }

        private string GetBenefitValueColorText(int targetTier)
        {
            // if(targetTier < meTier.value)
            //     return StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_COLOR_PREV_TIER"); 
            // if(targetTier > meTier.value)
            //     return StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_COLOR_NEXT_TIER"); 
            if (targetTier > meTier.value)
                return StringTableUtils.GetString(tableType, "VIP_BENEFITS_CELL_COLOR_NEXT_TIER");
            return "";
        }
    }
}
