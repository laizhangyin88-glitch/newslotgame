using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsMainChapterCellController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private GameObject caller;
        private Blackboard callerBB;
        private int chapter;

        private ContextElement chapterTextElement;
        private ContextElement comingSoonTextElement;

        private ContextElement selectedBaseElement;
        private ContextElement defaultBaseElement;

        private ContextElement masteredOutlineElement;

        private ContextElement unlockInfoTextElement;

        private bool isPlayChapterUnlockAnim = false;
        private bool isPlayStageUnlockAnim = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            caller = bb.GetValue<GameObject>("caller");
            callerBB = caller.GetComponent<Blackboard>();

            MetaContextElementUtils.SetClickable(root, OnClick);

            // Text
            chapterTextElement = ContextUtils.FindElement(root, "Text", CHILDREN);

            // Mastered
            masteredOutlineElement = ContextUtils.FindElement(root, "Mastered Outline", CHILDREN);

            // Base
            selectedBaseElement = ContextUtils.FindElement(root, "Base On", CHILDREN);
            defaultBaseElement = ContextUtils.FindElement(root, "Base Off", CHILDREN);

            // Coming
            comingSoonTextElement = ContextUtils.FindElement(root, "Text Coming", CHILDREN);
            MetaContextElementUtils.SetTextGlobal(comingSoonTextElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_COMING_SOON");

            // Unlock
            unlockInfoTextElement = ContextUtils.FindElement(root, "Unlock Information/Text", FULL);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_START_STAGE_UNLOCK_ANIM,
                () => isPlayStageUnlockAnim = true);

            Register(MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_STAGE_UNLOCK_ANIM,
                () => isPlayStageUnlockAnim = false);
        }

        public void SetUnlock(bool withAnim)
        {
            if (withAnim)
            {
                anim.SetBool("isActive", true);
                anim.SetBool("isLock", false);
                anim.SetBool("isUnlockock", true);
            }
            else
            {
                anim.SetBool("isLock", false);
                anim.SetBool("isUnlockock", false);
            }
        }

        public void SetLock()
        {
            anim.SetBool("isLock", true);
            anim.SetBool("isUnlockock", false);
        }

        public void SetMastered()
        {
            bb.SetValue("isMastered", true);
        }

        public IEnumerator OnUpdateChapterInfoCoroutine()
        {
            bool isSelected = bb.GetValue<bool>("isSelected");
            bool isNewUnlocked = bb.GetValue<bool>("isNewUnlocked");
            bool isLocked = bb.GetValue<bool>("isLocked");
            bool isComingSoon = bb.GetValue<bool>("isComingSoon");

            var isWaitStory = bb.GetValue<bool>("isWaitStory");
            var onFinishCutSceneTrigger = new EventTrigger(gameObject,
                MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_CUT_SCENE);

            // Thumbnail Image
            chapter = bb.GetValue<int>("chapter");
            string chapterName = HiddenObjects.Utils.ChapterNumberToSymbol(chapter);
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = chapterName + " Chapter Icon";
            if(!string.IsNullOrEmpty(chapterName))
            {
                MetaContextElementUtils.SimpleSetSprite(root, "Chapter Image", bundle, asset);
                MetaContextElementUtils.SimpleSetSprite(root, "Chapter Image Locked", bundle, asset);
            }

            // Selected
            MetaContextElementUtils.SetActive(selectedBaseElement, isSelected);
            MetaContextElementUtils.SetActive(defaultBaseElement, !isSelected);

            // Mastered
            bool isMastered = bb.GetValue<bool>("isMastered");
            MetaContextElementUtils.SetActive(masteredOutlineElement, isMastered);

            // Coming
            MetaContextElementUtils.SetActive(comingSoonTextElement, isComingSoon);
            MetaContextElementUtils.SetActive(chapterTextElement, !isComingSoon);
            if (!isComingSoon)
            {
                // Text
                MetaContextElementUtils.SetTextGlobal(chapterTextElement, "HIDDEN_OBJECTS_MAIN_SCENE_CHAPTER_TEXT", this.chapter);
            }

            // Wait Anim Ready
            yield return new WaitUntil(() => anim.enabled);

            // Play Unlock
            if (isSelected)
            {
                anim.SetBool("isActive", false);
            }
            if (isNewUnlocked)
            {
                SetAnim("isLocked");
            }
            else
            {
                // Lock / Unlock
                if (isComingSoon)
                {
                    anim.SetBool("isActive", false);
                    SetAnim("isComing");
                }
                else if (isLocked)
                {
                    anim.SetBool("isActive", false);
                    SetAnim("isLocked");
                }
                else SetAnim("");
            }

            // Wait Story
            if (isWaitStory)
            {
                yield return new WaitUntilTrigger(onFinishCutSceneTrigger);
            }

            // Highlight Unlock
            if (isNewUnlocked)
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                    HiddenObjects.Events.ON_START_CHAPTER_UNLOCK_ANIM);

                // Play Sounds
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_NEW_CHAPTER_UNLOCK).Play();

                isPlayChapterUnlockAnim = true;
                yield return new WaitForSeconds(0.35f);
                anim.SetBool("isActive", true);
                yield return new WaitForSeconds(1f);
                isPlayChapterUnlockAnim = false;

                // Unlock
                BlackboardUtils.SetOrCreateValue(bb, "isLocked", false);
                SetAnim("isUnlock");

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                    HiddenObjects.Events.ON_FINISH_CHAPTER_UNLOCK_ANIM);
            }

            bb.SetValue("isUpdated", false);
        }

        public void OnClick()
        {
            if (isPlayChapterUnlockAnim || isPlayStageUnlockAnim) return;

            bool isComingSoon = bb.GetValue<bool>("isComingSoon");
            if (isComingSoon) return;

            bool isSelected = bb.GetValue<bool>("isSelected");
            if (isSelected) return;

            bool isLocked = bb.GetValue<bool>("isLocked");
            bool isNewUnlocked = bb.GetValue<bool>("isNewUnlocked");
            if (isNewUnlocked || !isLocked)
            {
                var eventData = new EventData<int>(HiddenObjects.Events.ON_CLICK_CHAPTER, chapter - 1);
                EventSender.SendEvent(caller, eventData);
            }
            else // locked info
            {
                bool isPlayingChapterUnlockInfo = callerBB.GetValue<bool>("isPlayingChapterUnlockInfo");
                if (isPlayingChapterUnlockInfo) return;

                string causeText = "";

                bool isPrevChapterUnlock = bb.GetValue<bool>("isPrevChapterUnlock");
                bool isPrevChapterStageAllUnlock = bb.GetValue<bool>("isPrevChapterStageAllUnlock");
                bool isLevelLock = bb.GetValue<bool>("isLevelLock");
                int prevChapterCompltetedStarCount = bb.GetValue<int>("prevChapterCompltetedStarCount");
                int needStarCountForUnlock = HiddenObjects.Utils.NeedStarForChapterUnlock;

                if (!isPrevChapterUnlock)
                {
                    causeText = StringTableUtils.GetString(GLOBAL,
                        "HIDDEN_OBJECTS_CHAPTER_UNLOCK_INFO_UNLOCK_PREV_CHAPTER", chapter - 1);
                }
                else if (!isPrevChapterStageAllUnlock)
                {
                    causeText = StringTableUtils.GetString(GLOBAL,
                        "HIDDEN_OBJECTS_CHAPTER_UNLOCK_INFO_UNLOCK_ALL_STAGE", chapter - 1);
                }
                else if (prevChapterCompltetedStarCount < needStarCountForUnlock)
                {
                    int shortage = needStarCountForUnlock - prevChapterCompltetedStarCount;
                    causeText = StringTableUtils.GetString(GLOBAL,
                        "HIDDEN_OBJECTS_CHAPTER_UNLOCK_INFO_EARN_STAR", shortage, chapter - 1);
                }
                else if (isLevelLock)
                {
                    int level = bb.GetValue<int>("minLevel");
                    causeText = StringTableUtils.GetString(GLOBAL,
                        "HIDDEN_OBJECTS_CHAPTER_UNLOCK_INFO_LEVEL_UP", level);
                }

                MetaContextElementUtils.SetText(unlockInfoTextElement, causeText);

                anim.SetTrigger("isInformation");

                callerBB.SetValue("isPlayingChapterUnlockInfo", true);
            }
        }

        public void OnDisappearUnlockInfo()
        {
            callerBB.SetValue("isPlayingChapterUnlockInfo", false);
        }

        private void SetAnim(string key)
        {
            anim.SetBool("isLocked", key == "isLocked");
            anim.SetBool("isComing", key == "isComing");
            anim.SetBool("isUnlock", key == "isUnlock");
        }

#if DEV
        [Sirenix.OdinInspector.Button]
        private void TestChapterUnlock()
        {
            bb.SetValue("isComingSoon", false);
            bb.SetValue("isSelected", false);
            bb.SetValue("isNewUnlocked", true);
            bb.SetValue("isLocked", true);
            bb.SetValue("isWaitStory", false);

            StartCoroutine(OnUpdateChapterInfoCoroutine());
        }
#endif
    }
}
