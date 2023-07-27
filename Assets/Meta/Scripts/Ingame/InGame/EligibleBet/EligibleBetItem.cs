using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BagelCode
{
    public abstract class EligibleBetItem : MonoBehaviour
    {
        public struct BetTextInfo
        {
            public BetTextInfo(long _requireBet, bool _useThreshold, string _textKey)
            {
                requireBet = _requireBet;
                useThreshold = _useThreshold;
                textkey = _textKey;
            }
            public long requireBet;
            public bool useThreshold;
            public string textkey;
        }

        public enum DisplayType
        {
            None,
            Appear,
            Changed
        }

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected ContextElement root;
        protected Animator anim;

        protected string titleKey;
        protected string betKey;
        protected string betLessKey;

        protected List<BetTextInfo> betTextInfoList = new List<BetTextInfo>();

        protected bool isAvailable = false;
        protected bool isBetOver = false;
        protected bool isChanged = false;
        protected bool isDisplay = false;
        protected bool isBetEnough = false;
        protected bool isBonus = false;

        public virtual bool Init()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            return true;
        }

        protected abstract bool IsAvailableInternal();
        public abstract long GetThreshold();

        protected void SetDefaultStringTableKey(string title, string betLess, string bet = "")
        {
            titleKey = title;
            betLessKey = betLess;
            betKey = bet;
        }

        protected void SetStringTableKey(params BetTextInfo[] _betTextInfos)
        {
            betTextInfoList = _betTextInfos.ToList();
        }

        protected virtual void UpdateAll()
        {
            UpdateTitle();
            UpdateInfo();
            UpdateAnim();
        }

        protected virtual string GetTitle()
        {
            return StringTableUtils.GetString(StringTable.StringTableType.Global, titleKey);
        }

        protected virtual string GetInfo()
        {
            if (string.IsNullOrEmpty(betKey))
            {
                return StringTableUtils.GetString(GLOBAL, betLessKey);
            }
            else
            {
                string textKey = isBetEnough ? betKey : betLessKey;
                bool useThreshold = !isBetEnough;

                return useThreshold ?
                    StringTableUtils.GetString(GLOBAL, textKey, GetThreshold()) :
                    StringTableUtils.GetString(GLOBAL, textKey);
            }
        }

        public virtual void UpdateBet(long totalBet)
        {
            if (!IsAvailableInternal()) return;

            long minBet = GetThreshold();
            if (minBet == -1L) return;

            bool prevBetEnough = isBetEnough;

            isBetEnough = minBet <= totalBet;

            isBetOver = !prevBetEnough && isBetEnough;
            isChanged = prevBetEnough != isBetEnough;

            isBonus = BlackboardUtils.FindVariable<bool>("./isGameSpin")?.value ?? false;

            // 처음 On되거나,
            // 기준 뱃보다 낮으면 on
            isAvailable = isBetOver || totalBet < minBet && !isBonus;
        }

        public virtual void UpdateExtraBetIndex(int index) { }

        public void Appear()
        {
            // anim.SetTrigger(name);
            if (anim != null)
            {
                anim.SetBool("IsActive", true);
            }

            if (!isDisplay || isChanged)
            {
                UpdateAll();
            }

            isDisplay = true;
        }

        public void Disappear()
        {
            isDisplay = false;
            // SetAnimatorTrigger("Disappear");
            anim.SetBool("IsActive", false);
        }

        public bool IsAvailable()
        {
            return isAvailable;
        }

        protected virtual void UpdateTitle()
        {
            var text = GetTitle();
            MetaContextElementUtils.SimpleSetText(root, "Text Title", text);
        }

        protected virtual void UpdateInfo()
        {
            var text = GetInfo();
            MetaContextElementUtils.SimpleSetText(root, "Text Info", text);
        }

        protected virtual void UpdateAnim() { }
    }
}
