using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class HiddenObjectsPopupChapterRewardController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const string ON_COLLECT = "OnCollect";

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Collect
            var buttonCollectElement = ContextUtils.FindElement(root, "Button Collect", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonCollectElement, "Text", "BUTTON_COLLECT", CHILDREN);
            MetaContextElementUtils.SetClickable(buttonCollectElement, gameObject, ON_COLLECT, false);

            // Title
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text",
                "HIDDEN_OBJECTS_POPUP_CHAPTER_REWARD_TITLE", FULL);

            // Info
            int collectedStarCount = bb.GetValue<int>("starCount");
            int chapterNumber = bb.GetValue<int>("chapterNumber");
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Contents Area Full Text/Text",
                "HIDDEN_OBJECTS_POPUP_CHAPTER_REWARD_INFO", FULL, collectedStarCount, chapterNumber);

            // Effect
            var coinEffectElement = ContextUtils.FindElement(root, "Explosion Coin", CHILDREN);
            var gemEffectElement = ContextUtils.FindElement(root, "Explosion Gem", CHILDREN);
            var coinGemEffectElement = ContextUtils.FindElement(root, "Explosion Coin & Gem", CHILDREN);

            // Reward
            ContextElement itemElement1 = ContextUtils.FindElement(root, "Item 01", CHILDREN);
            ContextElement itemElement2 = ContextUtils.FindElement(root, "Item 02", CHILDREN);
            ContextElement plusElement = ContextUtils.FindElement(root, "Plus", CHILDREN);

            long rewardCoin = bb.GetVariable<long>("coin")?.value ?? 0L;
            long rewardGem = bb.GetVariable<long>("gem")?.value ?? 0L;
            string coinText = StringTableUtils.GetString(StringTable.StringTableType.Global,
                "HIDDEN_OBJECTS_CHAPTER_REWARD_POPUP_COIN", rewardCoin);
            string gemText = StringTableUtils.GetString(StringTable.StringTableType.Global,
                "HIDDEN_OBJECTS_CHAPTER_REWARD_POPUP_GEM", rewardGem);

            if (rewardCoin > 0L && rewardGem > 0L)
            {
                MetaContextElementUtils.SetActive(itemElement1, true);
                MetaContextElementUtils.SetActive(plusElement, true);
                MetaContextElementUtils.SetActive(itemElement2, true);

                MetaContextElementUtils.SetActive(coinGemEffectElement, true);

                MakeRewardObject(true, itemElement1, coinText);
                MakeRewardObject(false, itemElement2, gemText);
            }
            else if (rewardCoin > 0L)
            {
                MetaContextElementUtils.SetActive(itemElement1, true);
                MetaContextElementUtils.SetActive(plusElement, false);
                MetaContextElementUtils.SetActive(itemElement2, false);

                MetaContextElementUtils.SetActive(coinEffectElement, true);

                MakeRewardObject(true, itemElement1, coinText);
            }
            else if (rewardGem > 0L)
            {
                MetaContextElementUtils.SetActive(itemElement1, true);
                MetaContextElementUtils.SetActive(plusElement, false);
                MetaContextElementUtils.SetActive(itemElement2, false);

                MetaContextElementUtils.SetActive(gemEffectElement, true);

                MakeRewardObject(false, itemElement1, gemText);
            }
        }

        private void MakeRewardObject(bool isCoin, ContextElement itemElement, string value)
        {
            // Reward Text
            MetaContextElementUtils.SimpleSetText(itemElement, "Text Reward", value);

            string bundle = HiddenObjects.HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset;
            Transform parent = ContextUtils.FindElement(itemElement, "Image Area", CHILDREN).transform;
            if (isCoin)
            {
                asset = "Hidden Objects Image Reward Coin";
            }
            else // gem
            {
                asset = "Hidden Objects Image Reward Gem";
            }

            MetaObjectUtils.MakePrefab(bundle, asset, parent);
        }

        public void Close()
        {
            if (gameObject is null) return;

            EventSender.SendCalleeCallback(gameObject);
            anim.SetTrigger("Close");

            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
