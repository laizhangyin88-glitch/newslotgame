using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class EligibleBetVipPoint : EligibleBetItem
    {
        private const int MAX_ITEM_COUNT = 3;

        private bool[] isItemEnables = new bool[MAX_ITEM_COUNT];
        private ContextElement[] itemElements = new ContextElement[MAX_ITEM_COUNT];
        private Animator[] itemAnims = new Animator[MAX_ITEM_COUNT];
        private ContextElement[] itemLockedTextElements = new ContextElement[MAX_ITEM_COUNT];

        public override bool Init()
        {
            base.Init();

            SetDefaultStringTableKey(
                "VIP_LOUNGE_ELIGIBLE_ITEM_TITLE",
                "VIP_LOUNGE_ELIGIBLE_ITEM_BET_LESS");

            for (int i = 0; i < MAX_ITEM_COUNT; ++i)
            {
                itemElements[i] = ContextUtils.FindElement(root, "Item 0" + (i + 1), ContextSearchingType.ChildrenSearch);
                itemAnims[i] = itemElements[i].GetComponent<Animator>();

                MetaContextElementUtils.SimpleSetTextGlobal(itemElements[i], "Text Activated", "VIP_LOUNGE_ELIGIBLE_ITEM_ACTIVED", ContextSearchingType.ChildrenSearch);
                itemLockedTextElements[i] = ContextUtils.FindElement(itemElements[i], "Text Condition", ContextSearchingType.ChildrenSearch);
            }

            bool isVipLoungeEnabled = BlackboardQueryUtils.IsVipLoungeEnabled();
            isItemEnables[1] = isVipLoungeEnabled;
            itemElements[1].gameObject.SetActive(isVipLoungeEnabled);

            IsVipGameEnabled();

            return true;
        }

        public override long GetThreshold()
        {
            return 0L; // min bet 0
        }

        protected override bool IsAvailableInternal()
        {
            return BlackboardQueryUtils.IsVipLoungeEnabled();
        }

        public override void UpdateBet(long totalBet)
        {
            if (!IsAvailableInternal())
            {
                isAvailable = false;
                return;
            }

            long minBetMax = 0L;
            for(int i = 0; i < MAX_ITEM_COUNT; ++i)
            {
                bool isAvailableItem = IsAvailableItem(i);
                long minBet = isAvailableItem ? GetMinBet(i) : 0L;
                if(minBet > minBetMax)
                    minBetMax = minBet;
            }

            bool prevBetEnough = isBetEnough;
            isBetEnough = minBetMax <= totalBet; // 모든 아이템 획득 가능

            isChanged = true;

            isBonus = BlackboardUtils.FindVariable<bool>("./isGameSpin")?.value ?? false;

            isAvailable = !(prevBetEnough && isBetEnough) && !isBonus && BlackboardQueryUtils.IsVipLoungeEnabled();
        }

        protected override void UpdateAnim()
        {
            long totalBet = BlackboardQueryUtils.GetTotalBet();
            for (int i = 0; i < MAX_ITEM_COUNT; ++i)
            {
                bool isAvailableItem = IsAvailableItem(i);
                long minBet = GetMinBet(i);
                if (isAvailableItem && totalBet >= minBet)
                {
                    if (i == 1 && itemAnims[i].GetBool("Locked"))
                        GSManager.Instance.GetHandler("Bonus_Features_Notice_Open").Play();

                    MetaAnimatorUtils.SetAnimSafty(this, itemAnims[i], "Locked", false);
                    MetaAnimatorUtils.SetAnimSafty(this, itemAnims[i], "Unlock", true);
                }
                else
                {
                    if (i == 1 && itemAnims[i].GetBool("Unlock"))
                        GSManager.Instance.GetHandler("Bonus_Features_Notice_Lock").Play();

                    MetaAnimatorUtils.SetAnimSafty(this, itemAnims[i], "Locked", true);
                    MetaAnimatorUtils.SetAnimSafty(this, itemAnims[i], "Unlock", false);

                    MetaContextElementUtils.SetText(itemLockedTextElements[i],
                        GetItemLockedText(i, isAvailableItem, minBet));
                }
            }
        }

        private string GetItemLockedText(int i, bool isAvailableItem, long minBet)
        {
            if (isAvailableItem)
            {
                return StringTableUtils.GetString(GLOBAL, "VIP_LOUNGE_ELIGIBLE_ITEM_LOCKED", minBet);
            }
            else
            {
                if(i == 0)
                {
                    return StringTableUtils.GetString(GLOBAL, "VIP_LOUJNGE_ELIGIBLE_ITEM_JOIN_CLUB");
                }
            }

            return "";
        }

        private bool IsAvailableItem(int i)
        {
            if(i == 0) // LP
            {
                return ClubUtils.IsClubber();
            }
            else if(i == 1) // VLP
            {
                return true;
            }
            else if(i == 2) // Depot
            {
                return IsVipGameEnabled();
            }

            return true;
        }

        private long GetMinBet(int i)
        {
            if(i == 0) // LP
            {
                return BlackboardUtils.FindValue<long>("./lpEligibleBet");
            }
            else if(i == 1) // VLP
            {
                return BlackboardUtils.FindValue<long>("./metaEligibleBetThreshold/vipLounge/qualifiedBetAmount");
            }
            else if(i == 2) // Depot
            {
                return BlackboardUtils.FindValue<long>("./metaEligibleBetThreshold/vipLounge/buildDreamBetAmount");
            }

            return 0L;
        }

        private bool IsVipGameEnabled()
        {
            bool isVipGameEnabled = BlackboardQueryUtils.IsVegasDreamsActive();
            isItemEnables[2] = isVipGameEnabled;
            itemElements[2].gameObject.SetActive(isVipGameEnabled);
            return isVipGameEnabled;
        }
    }
}
