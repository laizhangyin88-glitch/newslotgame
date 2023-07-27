using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using UnityEngine.UI;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class CollectingGameInGameButtonController : MetaGameEventButtonController
    {
        private ContextElement collectingGameButtonElement;
        private Animator collectingGameButtonAnim = null;

        private float targetGauge;
        private float startGauge;
        private float waitTime = 0.1f;
        private ContextElement sliderElement;

        private float gauge = 0;

        private Animator packAnimator;
        private Animator iconAnimator;

        private void Start()
        {
            InitProperty();
        }

        public override void InitProperty()
        {
            if (isInit) return;

            base.InitProperty();

            if (collectingGameButtonElement == null)
                collectingGameButtonElement = ContextUtils.FindElement(root, "Collecting Game Button", ContextSearchingType.ChildrenSearch);

            ContextElement collectingGameIconElement = ContextUtils.FindElement(root, "Collecting Game Button/Icon Area/Collecting Game Icon", ContextSearchingType.FullNameSearch);
            iconAnimator = collectingGameIconElement.GetComponent<Animator>();
            packAnimator = ContextUtils.FindElement(root, "Collecting Game Bet Progress", ContextSearchingType.ChildrenSearch).GetComponent<Animator>();

            sliderElement = ContextUtils.FindElement(root, "Collecting Game Bet Progress/Bet Progress Bar", ContextSearchingType.FullNameSearch);

            collectingGameButtonAnim = collectingGameButtonElement.GetComponent<Animator>();
            collectingGameButtonAnim.SetBool("IsActive", true);

            UpdateProgressBar(false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_OPEN_META_EVENT_GROUP, OnOpenMetaEventGroup);
            Register(MetaEventDefine.ON_META_UI_EVENT, "OnLeaveMetaGame", UpdateBadge);

            isInit = true;
        }

        private void UpdateBadge()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            if (metaGameEnterInfo != null)
            {
                int possessions = metaGameEnterInfo.GetValue<int>("totalPossessions");
                int completeScratchers = metaGameEnterInfo.GetValue<int>("completeScratchers");
                int totalCount = possessions + completeScratchers;

                var badgeElement = ContextUtils.FindElement(collectingGameButtonElement, "Badge Area/Badge", ContextSearchingType.FullNameSearch);
                ContextElement tabBadgeTextElement = ContextUtils.FindElement(badgeElement, "Text", ContextSearchingType.ChildrenSearch);
                if (totalCount > 99)
                    MetaContextElementUtils.SetText(tabBadgeTextElement, "99+");
                else
                    MetaContextElementUtils.SetText(tabBadgeTextElement, totalCount.ToString());

                ContextAnimator contextAnimator = badgeElement as ContextAnimator;
                contextAnimator.isPreserve = true;
                contextAnimator.SetIntProperty(totalCount);

                collectingGameButtonElement.GetComponent<Animator>().SetBool("IsBadge", true);
            }
        }

        private void OnOpenMetaEventGroup()
        {
            packAnimator.SetBool("IsActive", false);
        }

        protected override ContextElement GetButtonClickable()
        {
            if (collectingGameButtonElement == null)
                collectingGameButtonElement = ContextUtils.FindElement(root, "Collecting Game Button", ContextSearchingType.ChildrenSearch);
            return collectingGameButtonElement;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (collectingGameButtonAnim != null)
                collectingGameButtonAnim.SetBool("IsActive", true);

            if (isInit)
            {
                UpdateLocked();
                UpdateBadge();
            }
        }

        protected override void OnEarnMetaGameItem()
        {
            EventSender.SendEvent(gameObject, "OnEarnItem");
        }

        protected override void UpdateMetaItemInfo()
        {
            UpdateBadge();
        }

        public void UpdateCollectingGameInGameProgressBar()
        {
            bool updateWhileActive = bb.GetVariable<bool>("_updateWhileActive")?.value ?? false;

            ActiveBalloon();
            UpdateProgressBar(updateWhileActive);

            if (updateWhileActive)
            {
                StartCoroutine(InGameProgressBarUpdateCoroutine());
            }
        }

        private void UpdateLocked()
        {
            float gaugeLevelAsFloat = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();
            iconAnimator.SetBool("IsLocked", gaugeLevelAsFloat < 1f);
        }

        private IEnumerator InGameProgressBarUpdateCoroutine()
        {
            float elapsed = 0f;
            while(elapsed < waitTime)
            {
                elapsed += Time.deltaTime;
                gauge = startGauge + elapsed / waitTime * (targetGauge - startGauge);
                sliderElement.GetComponent<ContextSlider>().SetFloatProperty(gauge);

                yield return new WaitForEndOfFrame();
            }

            gauge = targetGauge;
            sliderElement.GetComponent<ContextSlider>().SetFloatProperty(gauge);
        }

        private void UpdateProgressBar(bool updateWhileActive)
        {
            targetGauge = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();

            if (updateWhileActive)
            {
                startGauge = gauge;
            }
            else
            {
                sliderElement.GetComponent<ContextSlider>().SetFloatProperty(targetGauge);
                startGauge = targetGauge;
                gauge = targetGauge;
            }
        }

        private void ActiveBalloon()
        {
            packAnimator.SetBool("IsActive", true);

            float gaugeLevelAsFloat = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();

            for (int i = 1; i <= 4; i++)
            {
                if (i <= (int)gaugeLevelAsFloat)
                {
                    if (i == (int)gaugeLevelAsFloat)
                        UpdatePack(i, true, gaugeLevelAsFloat - System.Math.Truncate(gaugeLevelAsFloat));
                    else
                        UpdatePack(i, true, 0);
                }
                else
                {
                    UpdatePack(i, false, 0);
                }
            }

            iconAnimator.SetBool("IsLocked", gaugeLevelAsFloat < 1f);

            ContextElement textElement = ContextUtils.FindElement(root, "Collecting Game Bet Progress/Text", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(textElement, gaugeLevelAsFloat < 1f
                ? StringTableUtils.GetString(StringTable.StringTableType.Global, "COLLECTING_GAME_IN_GAME_INACTIVE_TEXT")
                : StringTableUtils.GetString(StringTable.StringTableType.Global, "COLLECTING_GAME_IN_GAME_ACTIVE_TEXT"));
        }

        private void UpdatePack(int index, bool isActive, double scale)
        {
            Color activeColor = Color.white;
            Color inActiveColor = new Color(0.39f, 0.39f, 0.39f);

            Vector3 bigScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 normalScale = new Vector3(0.8f, 0.8f, 0.8f);

            Vector3 chestScale = Vector3.Lerp(normalScale, bigScale, (float)scale);

            ContextElement packElement = ContextUtils.FindElement(root, string.Format("Collecting Game Bet Progress/Pack 0{0}", index), ContextSearchingType.FullNameSearch);
            packElement.GetComponent<RectTransform>().localScale = chestScale;
            packElement.GetComponent<Image>().color = isActive ? activeColor : inActiveColor;
        }
    }
}
