using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections.Generic;
using System.Collections;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsMainStageCellController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private GameObject caller;
        private int stage;

        private ContextElement playButtonElement;
        private ContextElement playButtonTextElement;
        private ContextElement starProgressBarElement;
        private ContextElement lockedStateElement;

        private ContextElement masteredElement;
        private ContextElement masteredOutlineElement;

        private int needFinderCount;
        
        private readonly List<float> starProgressList = new List<float>();

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            caller = bb.GetValue<GameObject>("caller");
            stage = bb.GetValue<int>("stage");

            // Set Star Progress
            float current = 0f;
            int counter = 0;
            while(current <= 0.99f)
            {
                starProgressList.Add(current);
                current += 0.02f;
                ++counter;
                if (counter == 8)
                {
                    current += 0.04f;
                    counter = 0;
                }
            }
            starProgressList.Add(1f);

            // Stage Number
            MetaContextElementUtils.SimpleSetText(root, "Text Stage", stage.ToString(), CHILDREN);

            // Star
            starProgressBarElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);

            // Play Button
            playButtonElement = ContextUtils.FindElement(root, "Button Play", CHILDREN);
            playButtonTextElement = ContextUtils.FindElement(playButtonElement, "Text", CHILDREN);
            MetaContextElementUtils.SetClickable(playButtonElement, OnClick);

            // Locked
            lockedStateElement = ContextUtils.FindElement(root, "Locked State", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(lockedStateElement, "Text Button",
                "HIDDEN_OBJECTS_MAIN_SCENE_STAGE_LOCKED", CHILDREN);

            // Mastered
            masteredElement = ContextUtils.FindElement(root, "Mastered", CHILDREN);
            masteredOutlineElement = ContextUtils.FindElement(root, "Mastered Outline", CHILDREN);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }

        public IEnumerator OnUpdateStageInfoCoroutine()
        {
            needFinderCount = bb.GetValue<int>("needFinderCount");
            bool isNewUnlocked = bb.GetValue<bool>("isNewUnlocked");
            bool isLocked = bb.GetValue<bool>("isLocked");
            bool showLockInfo = bb.GetValue<bool>("showLockInfo");

            var isWaitStory = bb.GetValue<bool>("isWaitStory");
            var onFinishCutSceneTrigger = new EventTrigger(gameObject,
                MetaEventDefine.ON_META_UI_EVENT, HiddenObjects.Events.ON_FINISH_CUT_SCENE);

            // Finder Consume Text
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Finder Use", "HIDDEN_OBJECTS_MAIN_STAGE_CONSUME_FINDER_TEXT", CHILDREN, needFinderCount);

            // Thumbnail Image
            int chapter = bb.GetValue<int>("chapter");
            string bundle = HiddenObjects.Defines.CONTENTS_BUNDLE;
            string asset = "Thumbnail " + HiddenObjects.Utils.GetStageAssetName(chapter, stage);
            MetaContextElementUtils.SimpleSetSprite(root, "Thumbnail Image", bundle, asset);

            // Star Progress
            int completedStarCount = bb.GetValue<int>("completedStarCount");
            int ongoingStarPercentile = bb.GetValue<int>("ongoingStarPercentile");
            int maxProgress = HiddenObjects.Defines.MAX_STAR_COUNT * 100;
            int iProgress = completedStarCount * 100 + ongoingStarPercentile;
            float progress = (float)iProgress / maxProgress;
            float modified = GetModifiedProgress(progress);
            MetaContextElementUtils.SetSliderValue(starProgressBarElement, modified);

            // Mastered
            bool isMastered = bb.GetValue<bool>("isMastered");
            MetaContextElementUtils.SetActive(masteredElement, isMastered);
            MetaContextElementUtils.SetActive(masteredOutlineElement, isMastered);

            // Play Button
            MetaContextElementUtils.SetTextGlobal(playButtonTextElement,
                "HIDDEN_OBJECTS_MAIN_SCENE_STAGE_FINDER_COUNT", needFinderCount);

            // Lock Info
            var lockedInfoElement = ContextUtils.FindElement(root, "Locked State/Text Locked Info", FULL);
            MetaContextElementUtils.SetActive(lockedInfoElement, showLockInfo);
            MetaContextElementUtils.SimpleSetActive(root, "Locked State/Image Lock", !showLockInfo, FULL);
            MetaContextElementUtils.SetText(lockedInfoElement, "");
            if (showLockInfo)
            {
                int needStar = bb.GetValue<int>("needStar");
                if (needStar > 0)
                {
                    MetaContextElementUtils.SetTextGlobal(lockedInfoElement, "HIDDEN_OBJECTS_MAIN_SCENE_STAGE_LOCKED_INFO",
                        needStar, stage - 1);
                }
            }

            // Wait Anim Ready
            yield return new WaitUntil(() => anim.enabled);

            // Play Unlock
            if (isNewUnlocked)
            {
                SetAnim("isLocked");
            }
            else
            {
                // Lock / Unlock
                if (!showLockInfo && isLocked) SetAnim("isNotOpen");
                else if (isLocked) SetAnim("isLocked");
                else SetAnim("");
            }

            bool isButtonHighlight = bb.GetValue<bool>("isButton");
            anim.SetBool("isButton", isButtonHighlight);

            // Wait Story
            if (isWaitStory)
            {
                yield return new WaitUntilTrigger(onFinishCutSceneTrigger);
            }

            // Highlight Unlock
            if (isNewUnlocked)
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                    HiddenObjects.Events.ON_START_STAGE_UNLOCK_ANIM);

                // Play Sounds
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_NEW_STAGE_UNLOCK).Play();

                yield return new WaitForSeconds(0.35f);
                anim.SetBool("isActive", true);
                yield return new WaitForSeconds(1.5f);
                SetAnim("isUnlock");

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                    HiddenObjects.Events.ON_FINISH_STAGE_UNLOCK_ANIM);
            }

            bb.SetValue("isUpdated", false);
        }

        private void SetAnim(string key)
        {
            anim.SetBool("isLocked", key == "isLocked");
            anim.SetBool("isNotOpen", key == "isNotOpen");
            anim.SetBool("isUnlock", key == "isUnlock");
        }

        private float GetModifiedProgress(float progress)
        {
            int count = starProgressList.Count;

            int index = (int)(progress * count);
            index = Mathf.Min(count - 1, index);

            if (index == 0)
            {
                if (progress > 0f)
                    return starProgressList[1];
            }
            else if (index == count - 1)
            {
                if (progress < 1f)
                    return starProgressList[count - 2];
            }

            return starProgressList[index];
        }

        private void OnClick()
        {
            int finder = HiddenObjects.Utils.Finder;
            if(finder >= needFinderCount)
            {
                // Play Sound
                GSManager.Instance.GetHandler(HiddenObjects.Defines.SOUNDS_CONSUME_FINDER).Play();
                StartCoroutine(PlayCoroutine());
            }
            else
            {
                // Play Sound
                GSManager.Instance.GetHandler("UI_Button_Normal").Play();
                EventSender.SendEvent(caller, HiddenObjects.Events.ON_CLICK_FINDER_SHOP);
            }
        }

        private IEnumerator PlayCoroutine()
        {
            // Play Finder Consume Anim
            anim.SetTrigger("isStart");

            // Disable Meta Interactables
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.INACTIVE_META_UI);

            // Update Finder Gauge
            int finder = HiddenObjects.Utils.Finder;
            HiddenObjects.Utils.UpdateFinderCount(finder - needFinderCount);

            var onFinderConsumedTrigger = new EventTrigger(gameObject, HiddenObjects.Events.ON_FINDER_CONSUMED);
            yield return new WaitUntilTrigger(onFinderConsumedTrigger);

            var clickEventData = new EventData<int>(HiddenObjects.Events.ON_CLICK_STAGE, stage - 1);
            EventSender.SendEvent(caller, clickEventData);
        }

#if DEV
        [Sirenix.OdinInspector.Button]
        private void TestStageUnlock()
        {
            bb.SetValue("isNotOpen", false);
            bb.SetValue("showLockInfo", false);
            bb.SetValue("isNewUnlocked", true);
            bb.SetValue("isLocked", true);
            bb.SetValue("isWaitStory", false);

            StartCoroutine(OnUpdateStageInfoCoroutine());
        }
#endif
    }
}
